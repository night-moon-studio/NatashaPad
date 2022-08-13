// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad;

public interface IReferenceResolver
{
    string ReferenceType { get; }

    Task<IList<PortableExecutableReference>> Resolve(string reference, CancellationToken cancellationToken = default);
}
