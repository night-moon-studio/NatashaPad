using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NatashaPad.MvvmServices.MessageBox;
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

            services.TryAddTransient<MainWindow>();
            services.TryAddTransient<MainViewModel>();
        }

        private void Init()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            DependencyResolver.SetDependencyResolver(services);

            Show(services.BuildServiceProvider());
        }

        private void Show(IServiceProvider serviceProvider)
        {
            var view = serviceProvider.GetRequiredService<MainWindow>();
            var vm = serviceProvider.GetRequiredService<MainViewModel>();
            view.DataContext = vm;
            view.Show();
        }
    }
}