using NatashaPad.MvvmBase;
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
        public UsingManageViewModel(CommonParam commonParam,
            IEnumerable<string> namespaces) : base(commonParam)
        {
            AllItems = new RemovableCollection<NamespaceItem>();
            foreach (var name in namespaces)
            {
                AllItems.Add(new NamespaceItem(name));
            }

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
        public NamespaceItem() { }

        public NamespaceItem(string name)
        {
            _namespace = name;
        }

        private string _namespace;
        public string Namespace
        {
            get => _namespace;
            set => SetProperty(ref _namespace, value);
        }
    }
}
