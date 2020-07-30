using MediatR;

using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using NatashaPad.MvvmServices.MessageBox;

namespace NatashaPad.ViewModels.Base
{
    public abstract class ViewModelBase : BindableBase
    {
        protected readonly CommonParam commonParam;

        protected ViewModelBase(CommonParam commonParam)
        {
            this.commonParam = commonParam;
        }

        protected IMediator Mediator => commonParam.Mediatr;
        protected Dispatcher Dispatcher => commonParam.Dispatcher;
        protected IServiceProvider ServiceProvider => commonParam.ServiceProvider;
        public T GetService<T>() => ServiceProvider.GetService<T>();

        protected void ShowMessage(string message)
        {
            Mediator.Publish(new MessageNotification(message));
        }
    }
}
