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
    internal static class NugetHelper
    {
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

        public static async Task<bool> EnsurePackageInstalled(string packageName, string packageVersion, string globalPackagesFolder = null)
        {
            if (string.IsNullOrEmpty(globalPackagesFolder))
            {
                globalPackagesFolder = GetGlobalPackagesFolder();
            }
            var packageDir = Path.Combine(globalPackagesFolder, packageName.ToLowerInvariant(),
                packageVersion.ToLowerInvariant());
            if (Directory.Exists(packageDir))
            {
                return true;
            }

            var logger = NullLogger.Instance;
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

            var pkgDownloadContext = new PackageDownloadContext(cache);
            var downloadRes = await repository.GetResourceAsync<DownloadResource>();

            await RetryHelper.TryInvokeAsync(async () =>
                await downloadRes.GetDownloadResourceResultAsync(
                    new PackageIdentity(packageName, new NuGetVersion(packageVersion)),
                    pkgDownloadContext,
                    globalPackagesFolder,
                    logger,
                    CancellationToken.None), r => true);

            return Directory.Exists(packageDir);
        }
    }
}