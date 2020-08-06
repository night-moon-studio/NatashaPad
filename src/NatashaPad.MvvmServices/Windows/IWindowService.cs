using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.MvvmServices.Windows
{
    public interface IWindowService
    {
        void Show<TViewModel>(TViewModel viewModel);
    }

    public interface IDialogService : IWindowService
    {
        void ShowDialog<TViewModel>(TViewModel viewModel);
    }
}
