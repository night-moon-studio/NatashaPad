using NatashaPad.MvvmBase;
using NatashaPad.ViewModels.Base;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using WeihanLi.Extensions;

namespace NatashaPad.ViewModels
{
    //TODO: 添加时，1. 获取光标；2. 滚动到空处；3. 空处红框显示；
    internal class UsingManageViewModel : DialogViewModelBase
    {
        public UsingManageViewModel(CommonParam commonParam,
            IEnumerable<string> namespaces) : base(commonParam)
        {
            AllItems = new RemovableCollection<NamespaceItem>();
            namespaces.Distinct()
                .ForEach(x => AllItems.Add(new NamespaceItem(x)));

            AddCommand = new DelegateCommand(Add);
        }

        public ICommand AddCommand { get; }
        private void Add()
        {
            if (!AllItems.Any(x => x.IsEmpty))
                AllItems.Add(new NamespaceItem());
        }

        public RemovableCollection<NamespaceItem> AllItems { get; }

        protected override Task OkAsync()
        {
            var empties = AllItems.Where(x => x.IsEmpty);
            var duplicates = AllItems.GroupBy(x => x.Namespace)
                .SelectMany(x => x.Skip(1));

            empties.Concat(duplicates)
                .ToArray()
                .ForEach(x => AllItems.Remove(x));
            return base.OkAsync();
        }
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

        internal bool IsEmpty => string.IsNullOrWhiteSpace(_namespace);

        public override bool Equals(object obj)
        {
            return obj is NamespaceItem item &&
                   _namespace == item._namespace;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_namespace);
        }
    }
}
