using System;

namespace NewMvvm
{
    public interface IViewTypeInfoLocator
    {
        Type GetView(Type viewModelType);

        ViewInfo GetViewInfo(Type viewType);
    }

    public interface IViewInstanceLocator
    {
        object GetView(Type viewModelType);
    }

    public static class ViewContainerExtensions
    {
        public static object GetView<TViewModel>(IViewInstanceLocator locator)
        {
            return locator.GetView(typeof(TViewModel));
        }
    }
}
