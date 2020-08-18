using NatashaPad.ReferenceResolver.Nuget;
using NatashaPad.ViewModels.Base;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using static NatashaPad.ViewModels.NugetManageViewModel;

namespace NatashaPad.ViewModels
{
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
            _installedPackages = Array.Empty<NugetReferenceResolver>();

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
            string input = _input?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            Output = string.Empty;

            if (_installedPackages != null && _installedPackages.Count > 0)
            {
                foreach (var package in _installedPackages)
                {
                    _scriptOptions.ReferenceResolvers.Add(new NugetReferenceResolver(package.PackageName, package.PackageVersion));
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
                    object result = await _scriptEngine.Eval(input, _scriptOptions);

                    if (null == result)
                    {
                        Output += "(null)";
                    }
                    else
                    {
                        string dumpedResult = _dumperResolver.Resolve(result.GetType())
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

        /// <summary>
        /// 命名空间
        /// </summary>
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

        private ICollection<NugetReferenceResolver> _installedPackages;

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
                return _installedPackages.Select(x =>
                    new InstalledPackage(x.PackageName, x.PackageVersion))
                    .ToArray();
            }

            ICollection<NugetReferenceResolver> GetUpdatedResolvers()
            {
                return vm.InstalledPackages.Select(x => new NugetReferenceResolver(x.Name, x.Version.ToString()))
                    .ToArray();
            }
        }
    }
}