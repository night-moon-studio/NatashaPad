using Microsoft.CodeAnalysis;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace NatashaPad.ReferenceResolver.Nuget
{
    public class NugetReferenceResolver : IReferenceResolver
    {
        public string PackageName { get; }
        public string PackageVersion { get; }

        public NugetReferenceResolver(string packageName, string packageVersion)
        {
            PackageName = packageName;
            PackageVersion = packageVersion;
        }

        public string ReferenceType => "NugetReference";

        public async Task<PortableExecutableReference[]> Resolve()
        {
            var globalPackagesFolder = NugetHelper.GetGlobalPackagesFolder();
            var packageDir = Path.Combine(globalPackagesFolder, PackageName.ToLowerInvariant(),
                PackageVersion.ToLowerInvariant());
            if (!Directory.Exists(packageDir))
            {
                var logger = NullLogger.Instance;
                var cache = new SourceCacheContext();
                var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

                var pkgDownloadContext = new PackageDownloadContext(cache);
                var downloadRes = await repository.GetResourceAsync<DownloadResource>();

                await RetryHelper.TryInvokeAsync(async () =>
                    await downloadRes.GetDownloadResourceResultAsync(
                        new PackageIdentity(PackageName, new NuGetVersion(PackageVersion)),
                        pkgDownloadContext,
                        globalPackagesFolder,
                        logger,
                        CancellationToken.None), r => true);
            }

            if (!Directory.Exists(packageDir))
            {
                throw new ApplicationException($"download nuget package({PackageName}:{PackageVersion}) failed");
            }
            // netcoreapp3.1
            var packagePath = Path.Combine(packageDir, "lib", "netcoreapp3.1", $"{PackageName}.dll");
            if (File.Exists(packagePath))
            {
                return new[] { MetadataReference.CreateFromFile(packagePath) };
            }

            // netcoreapp3.0
            packagePath = Path.Combine(packageDir, "lib", "netcoreapp3.0", $"{PackageName}.dll");
            if (File.Exists(packagePath))
            {
                return new[] { MetadataReference.CreateFromFile(packagePath) };
            }

            // netstandard2.1
            packagePath = Path.Combine(packageDir, "lib", "netstandard2.1", $"{PackageName}.dll");
            if (File.Exists(packagePath))
            {
                return new[] { MetadataReference.CreateFromFile(packagePath) };
            }

            // netstandard2.0
            packagePath = Path.Combine(packageDir, "lib", "netstandard2.0", $"{PackageName}.dll");
            if (File.Exists(packagePath))
            {
                return new[] { MetadataReference.CreateFromFile(packagePath) };
            }

            throw new InvalidOperationException("no supported target framework support");
        }
    }
}