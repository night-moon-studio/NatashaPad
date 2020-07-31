using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.MvvmServices
{
    internal class DefaultViewContainer : IViewContainer, IViewLocator, IViewInstanceLocator
    {
        private readonly IServiceCollection services;
        private readonly Dictionary<Type, Type> map;

        public DefaultViewContainer(IServiceCollection services)
        {
            this.services = services;

            map = new Dictionary<Type, Type>();
        }

        

        public Type GetView<TViewModel>()
        {
            if (!map.TryGetValue(typeof(TViewModel), out var viewType))
            {
                throw new KeyNotFoundException(
                    string.Format(Properties.Resource.CannotFindMatchedViewTypeOfFormatString,
                    typeof(TViewModel).Name));
            }

            return viewType;
        }

        public void Register<TView, TViewModel>() where TView:class
        {
            Register(typeof(TViewModel), typeof(TView));
            services.TryAddTransient<TView>();
        }

        private void Register(Type viewModelType, Type viewType)
        {
            map[viewModelType] = viewType;
        }
    }

    internal class DefaultViewLocator : IViewInstanceLocator
    {
        //public object GetView<TViewModel>(TViewModel vm)
        //{
        //    if (!map.TryGetValue(typeof(TViewModel), out var viewType))
        //    {
        //        throw new KeyNotFoundException(
        //            string.Format(Properties.Resource.CannotFindMatchedViewTypeOfFormatString,
        //            typeof(TViewModel).Name));
        //    }

        //    return serviceProvider.GetService(viewType);
        //}
    }
}
