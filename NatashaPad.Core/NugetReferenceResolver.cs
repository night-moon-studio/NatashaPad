using Microsoft.CodeAnalysis;
using System;
using System.Threading.Tasks;

namespace NatashaPad
{
    public class NugetReferenceResolver : IReferenceResolver
    {
        private readonly string _packageName;
        private readonly string _packageVersion;

        public NugetReferenceResolver(string packageName, string packageVersion)
        {
            _packageName = packageName;
            _packageVersion = packageVersion;
        }

        public Task<MetadataReference> Resolve()
        {
            throw new NotImplementedException();
        }
    }
}