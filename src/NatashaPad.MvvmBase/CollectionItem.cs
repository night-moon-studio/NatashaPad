using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace NatashaPad.MvvmBase
{
    public class CollectionItem : BindableBase
    {
        public event EventHandler NeedDeleteMe;

        private ICommand _deleteThisCommand;
        public ICommand DeleteThisCommand => _deleteThisCommand ??= new DelegateCommand(FireDeleteMe);
        private void FireDeleteMe() => NeedDeleteMe?.Invoke(this, EventArgs.Empty);
    }

    public class RemovableCollection<T> : ObservableCollection<T>
        where T : CollectionItem
    {
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            item.NeedDeleteMe += Item_NeedDeleteMe;
        }

        protected virtual void Item_NeedDeleteMe(object sender, EventArgs e)
        {
            Remove((T)sender);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            item.NeedDeleteMe -= Item_NeedDeleteMe;
        }
    }
}
