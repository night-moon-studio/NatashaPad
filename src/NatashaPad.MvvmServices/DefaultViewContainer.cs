
using System;
using System.Collections.Generic;
using System.Linq;

namespace NatashaPad.MvvmServices
{
    internal class DefaultViewContainer : IViewContainer, IViewLocator
    {
        private readonly Dictionary<Type, Type> map;

        public DefaultViewContainer()
        {
            map = new Dictionary<Type, Type>();
        }

        public DefaultViewContainer(IEnumerable<Tuple<Type, Type>> tuples)
        {
            map = tuples.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public Type GetView(Type vmType)
        {
            if (!map.TryGetValue(vmType, out Type viewType))
            {
                throw new KeyNotFoundException(
                    string.Format(Properties.Resource.CannotFindMatchedViewTypeOfFormatString,
                    vmType.Name));
            }

            return viewType;
        }

        public void Register<TView, TViewModel>() where TView : class
        {
            Register(typeof(TViewModel), typeof(TView));
        }

        private void Register(Type viewModelType, Type viewType)
        {
            map[viewModelType] = viewType;
        }
    }

    internal class DefaultViewLocator : IViewInstanceLocator
    {
        private readonly IViewLocator viewLocator;
        private readonly IServiceProvider serviceProvider;

        public DefaultViewLocator(
            IViewLocator viewLocator,
            IServiceProvider serviceProvider)
        {
            this.viewLocator = viewLocator;
            this.serviceProvider = serviceProvider;
        }

        public object GetView(Type type)
        {
            return serviceProvider.GetService(viewLocator.GetView(type));
        }
    }
}
