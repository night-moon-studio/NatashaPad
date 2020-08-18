using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using NatashaPad.MvvmBase;
using NatashaPad.ReferenceResolver.Nuget;
using NatashaPad.ViewModels.Base;

using NuGet.Versioning;

using Prism.Commands;

using WeihanLi.Extensions;

namespace NatashaPad.ViewModels
{
    //TODO: 界面加载后即激活搜索框
    internal partial class NugetManageViewModel : DialogViewModelBase
    {
        public NugetManageViewModel(CommonParam commonParam,
            IEnumerable<InstalledPackage> installedPackages) : base(commonParam)
        {
            InstalledPackages = new RemovableCollection<InstalledPackage>();
            installedPackages.ForEach(x => InstalledPackages.Add(x));

            SearchedPackages = new ObservableCollection<SearchedPackage>();

            SearchCommand = new DelegateCommand(async () => await SearchAsync());
        }

        protected override Task OkAsync()
        {
            return base.OkAsync();
        }

        public ObservableCollection<InstalledPackage> InstalledPackages { get; }
        public ObservableCollection<SearchedPackage> SearchedPackages { get; }


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
            if (string.IsNullOrWhiteSpace(text))
                return;
            text = text.Trim();

            //TODO: 这边都给了默认值。需要在界面上支持用户选择
            var packagesNames = await NugetHelper.GetPackages(text, true, default);

            SearchedPackages.Clear();
            foreach (var name in packagesNames)
            {
                var versions = await NugetHelper.GetPackageVersions(name, default);
                var pkg = new SearchedPackage(name,
                    versions.Select(x => x.ToString()).ToArray());
                pkg.InstallCommand = new DelegateCommand(
                    () => InstallPackage(pkg),
                    () => CanInstallPackage(pkg));

                SearchedPackages.Add(pkg);

                void InstallPackage(SearchedPackage package)
                {
                    var old = InstalledPackages.Where(x => x.Name == package.Name).SingleOrDefault();
                    if (old != default)
                    {
                        old.Version = package.SelectedVersion;
                    }
                    else
                    {
                        InstalledPackages.Insert(0, new InstalledPackage(package));
                    }
                }

                bool CanInstallPackage(SearchedPackage package)
                {
                    return package.SelectedVersion != default;
                }
            }
        }
    }
}
