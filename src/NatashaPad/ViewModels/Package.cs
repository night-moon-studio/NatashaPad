using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using NatashaPad.Mvvm;

using NuGet.Versioning;

using Prism.Mvvm;

namespace NatashaPad.ViewModels
{
    internal partial class NugetManageViewModel
    {
        internal interface IPackage
        {
            string Name { get; }
        }

        internal class SearchedPackage : BindableBase, IPackage
        {
            public string Name { get; }
            public SearchedPackage(string name,
                IEnumerable<string> versions)
            {
                Name = name;
                Versions = versions.Reverse()
                    .ToArray();
                selectedVersion = Versions.FirstOrDefault();
            }

            public IEnumerable<string> Versions { get; }

            private string selectedVersion;
            public string SelectedVersion
            {
                get => selectedVersion;
                set => SetProperty(ref selectedVersion, value);
            }

            public ICommand InstallCommand { get; internal set; }
        }

        internal class InstalledPackage : CollectionItem, IPackage
        {
            public string Name { get; }

            public InstalledPackage(string name,
                string version)
            {
                Name = name;
                Version = version;
            }

            public InstalledPackage(SearchedPackage searchedPackage)
                : this(searchedPackage.Name, searchedPackage.SelectedVersion)
            { }

            private string version;
            public string Version
            {
                get => version;
                internal set => SetProperty(ref version, value);
            }

            public ICommand UninstallCommand => DeleteThisCommand;
        }
    }
}
