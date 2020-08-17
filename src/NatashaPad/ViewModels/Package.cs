using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using NatashaPad.MvvmBase;

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
                IEnumerable<NuGetVersion> versions)
            {
                Name = name;
                Versions = versions.Reverse()
                    .Select(x => new VersionModel(x))
                    .ToArray();
                selectedVersion = Versions.FirstOrDefault();
            }

            public IEnumerable<VersionModel> Versions { get; }

            private VersionModel selectedVersion;
            public VersionModel SelectedVersion
            {
                get => selectedVersion;
                set => SetProperty(ref selectedVersion, value);
            }

            public ICommand InstallCommand { get; internal set; }

            public override bool Equals(object obj)
            {
                return obj is SearchedPackage package &&
                       Name == package.Name &&
                       EqualityComparer<VersionModel>.Default.Equals(SelectedVersion, package.SelectedVersion);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, SelectedVersion);
            }
        }

        internal class InstalledPackage : CollectionItem, IPackage
        {
            public string Name { get; }

            public InstalledPackage(string name,
                string version)
            {
                Name = name;
                Version = new VersionModel(version);
            }

            public InstalledPackage(string name,
                VersionModel version)
            {
                Name = name;
                Version = new VersionModel(version);
            }

            public InstalledPackage(SearchedPackage searchedPackage)
                : this(searchedPackage.Name, searchedPackage.SelectedVersion)
            { }

            private VersionModel version;
            public VersionModel Version
            {
                get => version;
                internal set => SetProperty(ref version, value);
            }

            public ICommand UninstallCommand => DeleteThisCommand;
        }

        internal class VersionModel
        {
            public VersionModel(NuGetVersion version)
            {
                InternalVersion = version;
            }

            public VersionModel(string version):this(new NuGetVersion(version))
            {
            }

            public VersionModel(VersionModel version) : this(version.InternalVersion)
            {
            }

            public NuGetVersion InternalVersion { get; }

            private string display;
            public string Display => display ??= ToString();

            public override bool Equals(object obj)
            {
                return obj is VersionModel model &&
                       Display == model.Display;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Display);
            }

            public override string ToString()
            {
                return InternalVersion.ToString();
            }
        }
    }
}
