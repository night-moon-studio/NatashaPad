using NatashaPad.ViewModels.Base;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NatashaPad.ViewModels
{
    internal class UsingManageViewModel : DialogViewModelBase
    {
        public UsingManageViewModel(CommonParam commonParam) : base(commonParam)
        {
            AllItems = new RemovableCollection<NamespaceItem>();
            AddCommand = new DelegateCommand(Add);
        }

        public ICommand AddCommand { get; }
        private void Add()
        {
            AllItems.Add(new NamespaceItem());
        }

        public RemovableCollection<NamespaceItem> AllItems { get; }
    }

    internal class NamespaceItem : CollectionItem
    {
        private string _namespace;
        public string Namespace
        {
            get => _namespace;
            set => SetProperty(ref _namespace, value);
        }

    }
}
