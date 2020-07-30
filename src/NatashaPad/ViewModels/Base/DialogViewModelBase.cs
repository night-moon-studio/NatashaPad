using Prism.Commands;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatashaPad.ViewModels.Base
{
    internal abstract class DialogViewModelBase : ViewModelBase
    {
        protected DialogViewModelBase(CommonParam commonParam) : base(commonParam)
        {
            OkCommand = new DelegateCommand(async () => await OkAsync(), CanOk);
            CancelCommand = new DelegateCommand(async () => await CancelAsync(), CanCancel);
        }

        public ICommand OkCommand { get; }
        protected abstract Task OkAsync();
        protected virtual bool CanOk() => true;

        public ICommand CancelCommand { get; }
        protected virtual Task CancelAsync() => Task.CompletedTask;
        protected virtual bool CanCancel() => true;
    }
}
