using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NatashaPad.MvvmServices.Windows
{
    internal class DefaultCurrentWindowService : ICurrentWindowService
    {
        private readonly Window window;

        public DefaultCurrentWindowService(Window window)
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
    }
}
