using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.MvvmServices
{
    public interface IViewContainer
    {
        void Register<TView, TViewModel>() where TView : class;
    }

    internal interface IViewLocator
    {
        Type GetView<TViewModel>();
    }

    public interface IViewInstanceLocator
    {
        object GetView<TViewModel>(TViewModel vm);
    }
}
