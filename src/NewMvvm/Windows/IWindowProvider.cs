using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NewMvvm.Windows
{
    public interface IWindowProvider
    {
        Window Create(object view, object viewModel);
    }
}
