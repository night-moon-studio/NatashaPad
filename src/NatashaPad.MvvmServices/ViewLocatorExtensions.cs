using Microsoft.Extensions.DependencyInjection.Extensions;

using NatashaPad.MvvmServices;
using NatashaPad.MvvmServices.Windows;

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
                services.AddTransient(item.Item1);
                services.AddTransient(item.Item2);
            }

            services.TryAddSingleton(new DefaultViewContainer(options));
            services.TryAddSingleton<IViewContainer>(s => s.GetService<DefaultViewContainer>());
            services.TryAddSingleton<IViewLocator>(s => s.GetService<DefaultViewContainer>());

            services.TryAddSingleton<IDialogService, DefaultDialogService>();

            services.TryAddSingleton<IViewInstanceLocator, DefaultViewLocator>();
        }
    }
}
