using Prism.Commands;
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

        /// <summary>
        /// 对话框的结果
        /// </summary>
        public bool Succeed { get; protected set; }

        public ICommand OkCommand { get; }

        protected virtual Task OkAsync()
        {
            Succeed = true;
            CloseMe();
            return Task.CompletedTask;
        }

        protected virtual bool CanOk() => true;

        public ICommand CancelCommand { get; }

        protected virtual Task CancelAsync()
        {
            CloseMe();
            return Task.CompletedTask;
        }

        protected virtual bool CanCancel() => true;
    }
}