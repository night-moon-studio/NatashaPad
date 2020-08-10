using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using MediatR;

namespace NatashaPad.MvvmServices.Windows
{
    public class DefaultWindowManager : IWindowManager
    {
        private readonly IViewInstanceLocator locator;
        private readonly IWindowProvider windowProvider;

        public DefaultWindowManager(
            IViewInstanceLocator locator,
            IWindowProvider windowProvider)
        {
            this.locator = locator;
            this.windowProvider = windowProvider;

            windowMap = new Dictionary<object, Window>();
        }

        private readonly Dictionary<object, Window> windowMap;
        public ICurrentWindowService GetCurrent<TViewModel>(TViewModel viewModel)
        {
            if (!windowMap.TryGetValue(viewModel, out var window))
            {
                throw new ArgumentException(Properties.Resource.FoundNoWindowErrorString);
            }

            return new DefaultCurrentWindowService(window);
        }

        public IDialogService GetDialogService<TViewModel>(TViewModel viewModel)
        {
            var view = locator.GetView(typeof(TViewModel));

            if (!(view is Window window))
            {
                window = windowProvider.Create(view, viewModel);
                window.Closed += Window_Closed;
            }

            windowMap[viewModel] = window;
            return new DefaultDialogService(window);

            void Window_Closed(object sender, EventArgs e)
            {
                windowMap.Remove(viewModel);
            }
        }

        public IWindowService GetWindowService<TViewModel>(TViewModel viewModel)
        {
            return GetDialogService(viewModel);
        }
    }
}
