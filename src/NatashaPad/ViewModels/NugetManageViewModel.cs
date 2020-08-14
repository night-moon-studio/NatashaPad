using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using NatashaPad.ReferenceResolver.Nuget;
using NatashaPad.ViewModels.Base;

using NuGet.Versioning;

using Prism.Commands;
using Prism.Mvvm;

namespace NatashaPad.ViewModels
{
    //TODO: 界面加载后即激活搜索框
    internal class NugetManageViewModel : DialogViewModelBase
    {
        public NugetManageViewModel(CommonParam commonParam) : base(commonParam)
        {
            InstalledPackages = new ObservableCollection<Package>();
            SearchedPackages = new ObservableCollection<Package>();

            SearchCommand = new DelegateCommand(async () => await SearchAsync());
        }

        protected override Task OkAsync()
        {
            return base.OkAsync();
        }

        public ObservableCollection<Package> InstalledPackages { get; }
        public ObservableCollection<Package> SearchedPackages { get; }


        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value);
        }

        public ICommand SearchCommand { get; }
        private async Task SearchAsync()
        {
            var text = searchText;

            //TODO: 这边都给了默认值。需要在界面上支持用户选择
            var packagesNames = await NugetHelper.GetPackages(text, true, default);

            SearchedPackages.Clear();
            foreach (var name in packagesNames)
            {
                var versions = await NugetHelper.GetPackageVersions(name, default);
                var pkg = new Package(name, versions);
                pkg.InstallCommand = new DelegateCommand(
                    () => InstallPackage(pkg),
                    () => CanInstallPackage(pkg));

                SearchedPackages.Add(pkg);

                void InstallPackage(Package package)
                {
                    var old = InstalledPackages.Where(x => x.Name == package.Name).SingleOrDefault();
                    if (old != default)
                    {
                        old.SelectedVersion = package.SelectedVersion;
                    }
                    else
                    {
                        InstalledPackages.Insert(0, package);
                    }
                }

                bool CanInstallPackage(Package package)
                {
                    return package.SelectedVersion != default;
                }
            }
        }

        internal class Package : BindableBase
        {
            public Package(string name,
                IEnumerable<NuGetVersion> versions)
            {
                Name = name;
                Versions = versions.Reverse()
                    .Select(x => new VersionModel(x))
                    .ToArray();
                selectedVersion = Versions.FirstOrDefault();
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
            public ICommand UninstallCommand { get; internal set; }

            public override bool Equals(object obj)
            {
                return obj is Package package &&
                       Name == package.Name &&
                       EqualityComparer<VersionModel>.Default.Equals(SelectedVersion, package.SelectedVersion);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, SelectedVersion);
            }
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

            public override bool Equals(object obj)
            {
                return obj is VersionModel model &&
                       Display == model.Display;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Display);
            }
        }
    }
}
