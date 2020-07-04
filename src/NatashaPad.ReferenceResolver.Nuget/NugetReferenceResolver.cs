using Microsoft.CodeAnalysis;
using System.Collections.Generic;
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

        public Task<IList<PortableExecutableReference>> Resolve()
        {
            return NugetHelper.ResolveAssemblies(PackageName, PackageVersion);
        }
    }
}