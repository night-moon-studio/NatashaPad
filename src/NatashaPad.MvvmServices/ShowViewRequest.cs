using MediatR;

using System;
using System.Collections.Generic;
using System.Text;

namespace NatashaPad.MvvmServices
{
    public class ShowViewRequest<TViewModel> : IRequest
    {
        public ShowViewRequest(TViewModel vm)
        {
            ViewModel = vm;
        }

        public TViewModel ViewModel { get; }
    }
}
