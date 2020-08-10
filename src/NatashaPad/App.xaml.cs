using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NatashaPad.MvvmServices.MessageBox;
using NatashaPad.MvvmServices.Windows;
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

            services.AddMediatR(typeof(App), typeof(MessageNotification));

            services.AddSingleton(Dispatcher.CurrentDispatcher);

            services.UsingViewLocator(options =>
            {
                options.Register<MainWindow, MainViewModel>();
                options.Register<UsingManageView, UsingManageViewModel>();
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