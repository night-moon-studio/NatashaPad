using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatashaPad
{
    public class FileReferenceResolver : IReferenceResolver
    {
        private readonly string _filePath;

        public FileReferenceResolver(string filePath)
        {
            _filePath = filePath;
        }

        public string ReferenceType => "FileReference";

        public async Task<IList<PortableExecutableReference>> Resolve()
        {
            var fileReference = MetadataReference.CreateFromFile(_filePath);
            return new[] { fileReference };
        }
    }
}