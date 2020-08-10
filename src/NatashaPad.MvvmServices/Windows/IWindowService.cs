using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.MvvmServices.Windows
{
    public interface IWindowManager
    {
        ICurrentWindowService GetCurrent<TViewModel>(TViewModel viewModel);
        IWindowService GetWindowService<TViewModel>(TViewModel viewModel);
        IDialogService GetDialogService<TViewModel>(TViewModel viewModel);
    }

    public interface IWindowService
    {
        void Show();
        void Hide();
        void Close();
    }

    public interface IDialogService : IWindowService
    {
        void ShowDialog();
    }

    public interface ICurrentWindowService
    {
        void Show();
        void Hide();
        void Close();
    }
}
