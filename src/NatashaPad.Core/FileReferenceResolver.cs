using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NatashaPad
{
    public class FileReferenceResolver : IReferenceResolver
    {
        private readonly string _filePath;

        public FileReferenceResolver(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public string ReferenceType => "FileReference";

        public Task<IList<PortableExecutableReference>> Resolve()
        {
            var fileReference = MetadataReference.CreateFromFile(_filePath);
            return Task.FromResult<IList<PortableExecutableReference>>(new[] { fileReference });
        }

        public override int GetHashCode()
        {
            return _filePath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _filePath.Equals((obj as FileReferenceResolver)?._filePath, StringComparison.OrdinalIgnoreCase);
        }
    }
}