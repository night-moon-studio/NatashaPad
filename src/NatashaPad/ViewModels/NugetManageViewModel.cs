using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using NatashaPad.ViewModels.Base;

using NuGet.Versioning;

using Prism.Commands;
using Prism.Mvvm;

namespace NatashaPad.ViewModels
{
    internal class NugetManageViewModel : DialogViewModelBase
    {
        public NugetManageViewModel(CommonParam commonParam) : base(commonParam)
        {
            InstalledPackages = new ObservableCollection<Package>();

            SearchCommand = new DelegateCommand(async () => await SearchAsync());
        }

        protected override Task OkAsync()
        {
            return base.OkAsync();
        }

        public ObservableCollection<Package> InstalledPackages { get; }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        public ICommand SearchCommand { get; }
        private async Task SearchAsync()
        {

        }

        internal class Package : BindableBase
        {
            public Package(string name,
                IEnumerable<NuGetVersion> versions)
            {
                Name = name;
                Versions = versions.Select(x => new VersionModel(x)).ToArray();
            }

            public string Name { get; }

            public IEnumerable<VersionModel> Versions { get; }

            private VersionModel selectedVersion;
            public VersionModel SelectedVersion
            {
                get => selectedVersion;
                set => SetProperty(ref selectedVersion, value);
            }

            public ICommand InstallCommand { get; internal set; }
        }

        internal class VersionModel
        {
            public VersionModel(NuGetVersion version)
            {
                Core = version;
            }

            public NuGetVersion Core { get; }

            private string display;
            public string Display => display ??= Core.ToString();
        }
    }
}
