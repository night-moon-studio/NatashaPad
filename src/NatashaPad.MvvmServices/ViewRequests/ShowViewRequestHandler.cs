using System.Windows;

using MediatR;

namespace NatashaPad.MvvmServices.ViewRequests
{
    public class ShowViewRequestHandler : RequestHandler<ShowViewRequest>
    {
        private readonly IViewInstanceLocator locator;

        public ShowViewRequestHandler(IViewInstanceLocator locator)
        {
            this.locator = locator;
        }

        protected override void Handle(ShowViewRequest request)
        {
            var window = new Window();
            window.Content = locator.GetView(request.Type);
            window.DataContext = request.ViewModel;
            window.ShowDialog();
        }
    }
}