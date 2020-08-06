using System;
using System.Windows.Threading;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using NatashaPad.MvvmServices.MessageBox;
using NatashaPad.MvvmServices.ViewRequests;
using NatashaPad.MvvmServices.Windows;

using Prism.Mvvm;

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
        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        protected void ShowMessage(string message)
        {
            Mediator.Publish(new MessageNotification(message));
        }

        protected IDialogService DialogService => commonParam.DialogService;
        protected T ShowDialog<T>() where T : ViewModelBase
        {
            T vm = GetService<T>();
            DialogService.ShowDialog(vm);
            return vm;
        }

        protected T ShowDialog<T>(T vm) where T : ViewModelBase
        {
            DialogService.ShowDialog(vm);
            return vm;
        }
    }
}
