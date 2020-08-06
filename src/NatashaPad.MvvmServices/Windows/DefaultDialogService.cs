using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

using NatashaPad.MvvmServices.ViewRequests;

namespace NatashaPad.MvvmServices.Windows
{
    internal class DefaultDialogService : IDialogService
    {
        private readonly IMediator mediator;

        public DefaultDialogService(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public void Show<TViewModel>(TViewModel viewModel)
        {
            ShowDialog(viewModel);
        }

        public void ShowDialog<TViewModel>(TViewModel viewModel)
        {
            mediator.Send(ShowViewRequest.Create(viewModel));
        }
    }
}
