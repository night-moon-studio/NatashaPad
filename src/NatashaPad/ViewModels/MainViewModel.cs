﻿// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using NatashaPad.ViewModels.Base;
using Prism.Commands;
using ReferenceResolver;
using System.Windows.Input;
using System.Windows.Threading;
using static NatashaPad.ViewModels.NugetManageViewModel;

namespace NatashaPad.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly INScriptEngine _scriptEngine;
    private readonly DumperResolver _dumperResolver;
    private readonly NScriptOptions _scriptOptions;

    public MainViewModel(DumperResolver dumperResolver,
        INScriptEngine scriptEngine,
        NScriptOptions scriptOptions,
        CommonParam commonParam) : base(commonParam)
    {
        _dumperResolver = dumperResolver;
        _scriptEngine = scriptEngine;
        _scriptOptions = scriptOptions;

        _namespaces = _scriptOptions.UsingList;
        _installedPackages = Array.Empty<NuGetReference>();

        _input = "\"Hello NatashaPad\"";

        DumpOutHelper.OutputAction += Dump;

        RunCommand = new DelegateCommand(async () => await RunAsync());
        UsingManageCommand = new DelegateCommand(UsingManageShow);
        NugetManageCommand = new DelegateCommand(NugetManageShow);
    }

    private void Dump(string content)
    {
        if (content is null)
        {
            return;
        }

        //https://stackoverflow.com/questions/1644079/change-wpf-controls-from-a-non-main-thread-using-dispatcher-invoke
        if (Dispatcher.CheckAccess())
        {
            Do();
        }
        else
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)Do);
        }

        void Do() => Output += $"{content}{Environment.NewLine}";
    }

    private string _input;

    public string Input
    {
        get => _input;
        set => SetProperty(ref _input, value);
    }

    private string _output;

    public string Output
    {
        get => _output;
        set => SetProperty(ref _output, value);
    }

    public ICommand RunCommand { get; }

    private async Task RunAsync()
    {
        var input = _input?.Trim();
        if (string.IsNullOrEmpty(input))
        {
            return;
        }
        Output = string.Empty;
        if (_namespaces is { Count: > 0 })
        {
            foreach (var ns in _namespaces)
            {
                _scriptOptions.UsingList.Add(ns);
            }
        }
        if (_installedPackages is { Count: > 0 })
        {
            foreach (var packageReference in _installedPackages)
            {
                _scriptOptions.References.Add(packageReference);
            }
        }
        try
        {
            if (input.Contains("static void Main(") || input.EndsWith(";"))
            {
                // statements, execute
                await _scriptEngine.Execute(input, _scriptOptions);
            }
            else
            {
                // expression, eval
                var result = await _scriptEngine.Eval(input, _scriptOptions);
                if (result is null)
                {
                    Output += "(null)";
                }
                else
                {
                    var dumpedResult = _dumperResolver.Resolve(result.GetType())
                        .Dump(result);
                    Output += dumpedResult;
                }
            }
        }
        catch (Exception exception)
        {
            ShowMessage(exception.Message);
        }
    }

    private ICollection<string> _namespaces;

    public ICommand UsingManageCommand { get; }

    private void UsingManageShow()
    {
        var vm = new UsingManageViewModel(commonParam, _namespaces);
        ShowDialog(vm);
        if (vm.Succeed)
        {
            _namespaces = vm.AllItems
                .Select(x => x.Namespace)
                .ToArray();
        }
    }

    private ICollection<NuGetReference> _installedPackages;

    public ICommand NugetManageCommand { get; }

    private void NugetManageShow()
    {
        var vm = new NugetManageViewModel(commonParam, GetInstalledPackages());
        ShowDialog(vm);
        if (vm.Succeed)
        {
            _installedPackages = GetUpdatedResolvers();
        }

        ICollection<InstalledPackage> GetInstalledPackages()
        {
            var packages = new InstalledPackage[_installedPackages.Count];
            var idx = 0;
            foreach(var package in _installedPackages)
            {
                var (packageId, packageVersion, _) = package;
                packages[idx] = new InstalledPackage(packageId, packageVersion); 
            }
            return packages;
        }

        ICollection<NuGetReference> GetUpdatedResolvers()
        {
            return vm.InstalledPackages.Select(x => new NuGetReference(x.Name, x.Version))
                .ToArray();
        }
    }
}
