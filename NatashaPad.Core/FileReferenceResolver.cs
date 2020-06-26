using Microsoft.CodeAnalysis;
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

        public Task<MetadataReference> Resolve()
        {
            var fileReference = MetadataReference.CreateFromFile(_filePath);
            return Task.FromResult((MetadataReference)fileReference);
        }
    }
}