using MediatR;

using NatashaPad.MvvmServices.Windows;

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
            IDialogService dialogService,
            Dispatcher dispatcher)
        {
            ServiceProvider = serviceProvider;
            Mediatr = mediatr;
            DialogService = dialogService;
            Dispatcher = dispatcher;
        }

        public IServiceProvider ServiceProvider { get; }
        public IMediator Mediatr { get; }
        public IDialogService DialogService { get; }
        public Dispatcher Dispatcher { get; }
    }
}
