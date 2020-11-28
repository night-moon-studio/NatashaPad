using Microsoft.Extensions.DependencyInjection.Extensions;

using NatashaPad.Mvvm;
using NatashaPad.Mvvm.MessageBox;
using NatashaPad.Mvvm.Windows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ViewLocatorExtensions
    {
        public static void UsingViewLocator(this IServiceCollection services,
            Action<ViewContainerOptions> action)
        {
            var options = new ViewContainerOptions();
            action?.Invoke(options);

            foreach (var item in options)
            {
                services.AddTransient(item.ViewType);
                services.AddTransient(item.ViewModelType);
            }

            services.TryAddSingleton(new DefaultViewContainer(options));
            services.TryAddSingleton<IViewTypeInfoLocator>(s => s.GetService<DefaultViewContainer>());

            services.TryAddSingleton<IWindowManager, DefaultWindowManager>();

            services.TryAddSingleton<IViewInstanceLocator, DefaultViewLocator>();

            services.TryAddSingleton<IWindowProvider, DefaultWindowProvider>();

            services.TryAddTransient<IErrorMessageBoxService, DefaultErrorMessageBoxService>();
        }
    }
}
