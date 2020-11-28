using System;
using System.Windows.Threading;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using NatashaPad.Mvvm.MessageBox;
using NatashaPad.Mvvm.Windows;

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
            GetService<IErrorMessageBoxService>().Show(message);
        }

        protected IWindowManager WindowManager => commonParam.WindowManager;
        protected T ShowDialog<T>() where T : ViewModelBase
        {
            return ShowDialog(GetService<T>());
        }

        protected T ShowDialog<T>(T vm) where T : ViewModelBase
        {
            WindowManager.GetDialogService(vm).ShowDialog();
            return vm;
        }

        protected ICurrentWindowService GetCurrentView => WindowManager.GetCurrent(this);

        protected void CloseMe() => GetCurrentView.Close();
    }
}
