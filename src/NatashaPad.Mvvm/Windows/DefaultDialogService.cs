using System.Windows;

namespace NatashaPad.Mvvm.Windows
{
    internal class DefaultDialogService : IDialogService, IWindowService
    {
        private readonly Window window;

        public DefaultDialogService(Window window)
        {
            this.window = window;
        }

        public void Close()
        {
            window.Close();
        }

        public void Hide()
        {
            window.Hide();
        }

        public void Show()
        {
            window.Show();
        }

        public void ShowDialog()
        {
            window.ShowDialog();
        }
    }
}