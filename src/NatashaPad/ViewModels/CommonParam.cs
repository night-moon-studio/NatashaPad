using MediatR;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace NatashaPad.ViewModels
{
    public sealed class CommonParam
    {
        public CommonParam(IServiceProvider serviceProvider,
            IMediator mediatr,
            Dispatcher dispatcher)
        {
            ServiceProvider = serviceProvider;
            Mediatr = mediatr;
            Dispatcher = dispatcher;
        }

        public IServiceProvider ServiceProvider { get; }
        public IMediator Mediatr { get; }
        public Dispatcher Dispatcher { get; }
    }
}
