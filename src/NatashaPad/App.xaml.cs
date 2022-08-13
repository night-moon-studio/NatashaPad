// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NatashaPad.Mvvm;
using NatashaPad.Mvvm.Windows;
using NatashaPad.ReferenceResolver.Nuget;
using NatashaPad.ViewModels;
using NatashaPad.Views;
using System.Windows;
using System.Windows.Threading;
using WeihanLi.Common;

namespace NatashaPad;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Init();
        base.OnStartup(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IReferenceResolver, FileReferenceResolver>();
        services.AddSingleton<IReferenceResolver, NuGetReferenceResolver>();
        services.AddSingleton<INScriptEngine, CSharpScriptEngine>();
        services.AddTransient<NScriptOptions>();
        services.TryAddSingleton<DumperResolver>();
        services.AddSingleton<IDumper, DefaultDumper>();

        services.AddTransient<CommonParam>();

        services.AddMediatR(typeof(App));

        services.AddSingleton(Dispatcher.CurrentDispatcher);

        services.UsingViewLocator(options =>
        {
            options.Register<MainWindow, MainViewModel>();
            options.Register<UsingManageView, UsingManageViewModel>(opt =>
            {
                opt.Width = 600;
                opt.Height = 400;
                opt.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                opt.Title = NatashaPad.Properties.Resource.UsingManageTitleString;
            });
            options.Register<NugetManageView, NugetManageViewModel>(opt =>
            {
                opt.Width = 800;
                opt.Height = 450;
                opt.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                opt.Title = NatashaPad.Properties.Resource.NugetManageTitleString;
            });
        });
    }

    private void Init()
    {
        IServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        DependencyResolver.SetDependencyResolver(services);

        var windowService = DependencyResolver.ResolveRequiredService<IWindowManager>()
            .GetWindowService(DependencyResolver.ResolveRequiredService<MainViewModel>());
        windowService.Show();
    }
}
