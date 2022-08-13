// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad;

public interface IReference
{
    string ReferenceType { get; }

    public string Reference { get; }
}

public sealed record FileReference : IReference
{
    public string ReferenceType => "file";
    public string Reference { get; }
    public FileReference(string filePath)
    {
        Reference = filePath;
    }
}

public sealed record NuGetReference : IReference
{
    public string ReferenceType { get; } = "nuget";
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
