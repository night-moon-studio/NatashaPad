using System.Windows;

using MediatR;

using NatashaPad.MvvmServices.Windows;

namespace NatashaPad.MvvmServices.ViewRequests
{
    public class ShowViewRequestHandler : RequestHandler<ShowViewRequest>
    {
        private readonly IViewInstanceLocator locator;
        private readonly IWindowProvider windowProvider;

        public ShowViewRequestHandler(IViewInstanceLocator locator,
            IWindowProvider windowProvider)
        {
            this.locator = locator;
            this.windowProvider = windowProvider;
        }

        protected override void Handle(ShowViewRequest request)
        {
            var view = locator.GetView(request.Type);
            var viewModel = request.ViewModel;
            var window = windowProvider.Create(view, viewModel);
            window.ShowDialog();
        }
    }
}