using System.Windows;

namespace NatashaPad.Mvvm.Windows;

public interface IWindowProvider
{
    Window Create(object view, object viewModel);
}