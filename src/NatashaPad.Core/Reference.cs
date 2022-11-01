// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using ReferenceResolver;

namespace NatashaPad;

public interface IReference
{
    ReferenceType ReferenceType { get; }

    public string Reference { get; }
}

public sealed record FileReference : IReference
{
    public ReferenceType ReferenceType => ReferenceType.LocalFile;
    public string Reference { get; }
    public FileReference(string filePath)
    {
        Reference = filePath;
    }
}

public sealed record NuGetReference : IReference
{
    public ReferenceType ReferenceType => ReferenceType.NuGetPackage;
    public string PackageId { get; set; }
    public string PackageVersion { get; }
    public string Reference { get; }

    public NuGetReference(string packageId, string packageVersion)
    {
        PackageId = packageId;
        PackageVersion = packageVersion;
        Reference = $"nuget:{packageId},{packageVersion}";
    }
}
