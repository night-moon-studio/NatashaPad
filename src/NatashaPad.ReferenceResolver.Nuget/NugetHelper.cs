using Microsoft.CodeAnalysis;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace NatashaPad.ReferenceResolver.Nuget
{
    public static class NugetHelper
    {
        private static readonly ILogger Logger = NullLogger.Instance;
        private static readonly SourceCacheContext Cache = new SourceCacheContext();
        private static readonly SourceRepository Repository = NuGet.Protocol.Core.Types.Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

        private const string DefaultTargetFramework = "netcoreapp3.1";

        private static readonly string GlobalPackagesFolder;

        public static string GetGlobalPackagesFolder()
        {
            using var executor = new ProcessExecutor("dotnet", "nuget locals global-packages -l");
            var folder = string.Empty;
            executor.OnOutputDataReceived += (sender, str) =>
            {
                if (str is null)
                    return;

                Console.WriteLine(str);

                if (str.StartsWith("global-packages:"))
                {
                    folder = str.Substring("global-packages:".Length).Trim();
                }
            };
            executor.Execute();

            return folder;
        }

        static NugetHelper()
        {
            GlobalPackagesFolder = GetGlobalPackagesFolder();
        }

        public static async Task<IEnumerable<string>> GetPackages(string packagePrefix, bool includePrerelease = true, CancellationToken cancellationToken = default)
        {
            var resource = await Repository.GetResourceAsync<AutoCompleteResource>(cancellationToken);
            var result = await resource.IdStartsWith(packagePrefix, includePrerelease, Logger, cancellationToken);
            return result;
        }

        public static async Task<IEnumerable<NuGetVersion>> GetPackageVersions(string packageId, CancellationToken cancellationToken = default)
        {
            var resource = await Repository.GetResourceAsync<FindPackageByIdResource>(cancellationToken);
            var result = await resource.GetAllVersionsAsync(packageId, Cache, Logger, cancellationToken);
            return result;
        }

        public static async Task<IList<PortableExecutableReference>> ResolveAssemblies(string packageName, string packageVersion)
        {
            var nugetVersion = NuGetVersion.Parse(packageVersion);
            var dependencies = await GetPackageDependencies(packageName, nugetVersion);
            if (dependencies.ContainsKey(packageName))
            {
                if (dependencies[packageName] < nugetVersion)
                {
                    dependencies[packageName] = nugetVersion;
                }
            }
            else
            {
                dependencies[packageName] = nugetVersion;
            }

            var packagePaths = await dependencies
                .Select(d => ResolvePackagePath(d.Key, d.Value))
                .WhenAll();
            //
            return packagePaths.Select(p => MetadataReference.CreateFromFile(p)).ToArray();
        }

        private static async Task<string> ResolvePackagePath(string packageId, NuGetVersion version)
        {
            var packageDir = Path.Combine(GlobalPackagesFolder, packageId.ToLowerInvariant(),
                version.ToString());
            if (!Directory.Exists(packageDir))
            {
                await EnsurePackageInstalled(packageId, version);
            }
            var findPkgByIdRes = await Repository.GetResourceAsync<FindPackageByIdResource>();
            var dependencyInfo = await findPkgByIdRes.GetDependencyInfoAsync(packageId, version, Cache, Logger,
                CancellationToken.None);
            var nearestFramework = dependencyInfo.DependencyGroups
                .GetBestDependency();
            if (null != nearestFramework)
            {
                var targetFrameworkString = nearestFramework.TargetFramework.GetFrameworkString();
                packageDir = Path.Combine(packageDir, "lib", targetFrameworkString.ToLowerInvariant().TrimStart('.'));

                var packagePath = Path.Combine(packageDir, $"{packageId}.dll");
                if (File.Exists(packagePath))
                {
                    return packagePath;
                }

                packagePath = Directory.GetFiles(packageDir, "*.dll", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (null != packagePath)
                {
                    return packagePath;
                }
            }

            throw new InvalidOperationException($"package({packageId}:{version}) cannot be used for({DefaultTargetFramework})");
        }

        public static async Task<Dictionary<string, NuGetVersion>> GetPackageDependencies(string packageName, NuGetVersion packageVersion)
        {
            var findPkgByIdRes = await Repository.GetResourceAsync<FindPackageByIdResource>();
            var dependencyInfo = await findPkgByIdRes.GetDependencyInfoAsync(packageName, new NuGetVersion(packageVersion), Cache, Logger,
                CancellationToken.None);
            if (dependencyInfo.DependencyGroups.Count > 0)
            {
                var nearestFramework = dependencyInfo.DependencyGroups
                    .GetBestDependency();
                if (null != nearestFramework)
                {
                    var list = new Dictionary<string, NuGetVersion>(StringComparer.OrdinalIgnoreCase);
                    foreach (var package in nearestFramework.Packages)
                    {
                        if (list.ContainsKey(package.Id))
                        {
                            if (list[package.Id] < package.VersionRange.MinVersion)
                            {
                                list[package.Id] = package.VersionRange.MinVersion;
                            }
                        }
                        else
                        {
                            list.Add(package.Id, package.VersionRange.MinVersion);
                        }
                        var childrenDependencies = await GetPackageDependencies(package.Id, package.VersionRange.MinVersion);
                        if (childrenDependencies != null && childrenDependencies.Count > 0)
                        {
                            foreach (var childrenDependency in childrenDependencies)
                            {
                                if (list.ContainsKey(childrenDependency.Key))
                                {
                                    if (list[childrenDependency.Key] < childrenDependency.Value)
                                    {
                                        list[childrenDependency.Key] = childrenDependency.Value;
                                    }
                                }
                                else
                                {
                                    list.Add(childrenDependency.Key, childrenDependency.Value);
                                }
                            }
                        }
                    }
                    return list;
                }
            }
            else
            {
                return new Dictionary<string, NuGetVersion>();
            }

            throw new InvalidOperationException($"no supported target framework for package({packageName}:{packageVersion})");
        }

        public static async Task<bool> EnsurePackageInstalled(string packageName, NuGetVersion nugetVersion)
        {
            var packageDir = Path.Combine(GlobalPackagesFolder, packageName.ToLowerInvariant(),
                nugetVersion.ToString());
            if (Directory.Exists(packageDir))
            {
                return true;
            }

            var packagerIdentity = new PackageIdentity(packageName, nugetVersion);

            var pkgDownloadContext = new PackageDownloadContext(Cache);
            var downloadRes = await Repository.GetResourceAsync<DownloadResource>();

            await RetryHelper.TryInvokeAsync(async () =>
                await downloadRes.GetDownloadResourceResultAsync(
                    packagerIdentity,
                    pkgDownloadContext,
                    GlobalPackagesFolder,
                    Logger,
                    CancellationToken.None), r => true);

            return Directory.Exists(packageDir);
        }

        private static PackageDependencyGroup GetBestDependency(this IReadOnlyList<PackageDependencyGroup> dependencyGroups)
        {
            var group = dependencyGroups.FirstOrDefault(x =>
                x.TargetFramework.GetFrameworkString().Equals(DefaultTargetFramework, StringComparison.OrdinalIgnoreCase));
            if (null != group)
            {
                return group;
            }

            group = dependencyGroups
                    .Where(x => x.TargetFramework.Framework.Equals(".netstandard", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(x => x.TargetFramework.Version)
                    .FirstOrDefault()
                ;
            if (null != group)
            {
                return group;
            }

            return null;
        }
    }
}