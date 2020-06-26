using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Windows;

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
            services.TryAddTransient<MainWindow>();
            services.AddSingleton<INScriptEngine, CSharpScriptEngine>();
            services.TryAddSingleton<DumperResolver>();
            services.AddSingleton<IDumper, DefaultDumper>();
        }

        private void Init()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);

            services.BuildServiceProvider()
                .GetRequiredService<MainWindow>()
                .Show();
        }
    }
}