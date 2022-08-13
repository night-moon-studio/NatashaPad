// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad;

public class FileReferenceResolver : IReferenceResolver
{
    public string ReferenceType => "file";

    public Task<IList<PortableExecutableReference>> Resolve(string reference, CancellationToken cancellationToken = default)
    {
        var filePath = reference ?? throw new ArgumentNullException(nameof(reference));
        var fileReference = MetadataReference.CreateFromFile(filePath);
        return Task.FromResult<IList<PortableExecutableReference>>(new[] { fileReference });
    }
}
