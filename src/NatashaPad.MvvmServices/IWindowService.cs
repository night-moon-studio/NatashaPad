using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.MvvmServices
{
    public interface IWindowService
    {
        TViewModel Show<TViewModel>();
    }

    public interface IDialogService : IWindowService
    {
        TViewModel ShowDialog<TViewModel>();
    }
}
