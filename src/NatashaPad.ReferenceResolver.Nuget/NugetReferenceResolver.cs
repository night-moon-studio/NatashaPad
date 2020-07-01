using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Threading.Tasks;

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

            if (!await NugetHelper.EnsurePackageInstalled(PackageName, PackageVersion, globalPackagesFolder))
            {
                throw new ApplicationException($"download nuget package({PackageName}:{PackageVersion}) failed");
            }

            // netcoreapp3.1
            var packagePath = Path.Combine(packageDir, "lib", "netcoreapp3.1", $"{PackageName}.dll");
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