using System;

namespace NatashaPad.MvvmServices
{
    public interface IViewContainer
    {
        void Register<TView, TViewModel>() where TView : class;
    }

    internal interface IViewLocator
    {
        Type GetView(Type viewModelType);
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
