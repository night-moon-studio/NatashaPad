using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NatashaPad.MvvmServices.Windows
{
    public interface IWindowManager
    {
        ICurrentWindowService GetCurrent<TViewModel>(TViewModel viewModel);
    }

    internal interface IWindowPool
    {
        void Add<TView, TViewModel>(Window window, TView view, TViewModel viewModel) where TViewModel : class;

        bool TryGetWindow<TViewModel>(TViewModel viewModel, out Window window) where TViewModel : class;

        void Remove(Window window);
    }
}
