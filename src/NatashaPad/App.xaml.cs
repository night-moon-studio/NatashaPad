using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NewMvvm.MessageBox;
using NewMvvm.Windows;
using NatashaPad.ViewModels;
using NatashaPad.Views;

using System;
using System.Windows;
using System.Windows.Threading;

using WeihanLi.Common;

namespace NatashaPad
{
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
                    opt.Title = "Using管理";
                });
                options.Register<NugetManageView, NugetManageViewModel>(opt =>
                {
                    opt.Width = 800;
                    opt.Height = 450;
                    opt.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    opt.Title = "引用管理";
                });
            });
        }

        private void Init()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            DependencyResolver.SetDependencyResolver(services);

            ShowWindow(services.BuildServiceProvider());
        }

        private void ShowWindow(IServiceProvider serviceProvider)
        {
            var vm = serviceProvider.GetRequiredService<MainViewModel>();

            var mgr = serviceProvider.GetRequiredService<IWindowManager>();
            var windowService = mgr.GetWindowService(vm);

            windowService.Show();
        }
    }
}