// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad.Test;
public class ReferenceTest
{
    [Fact]
    public void FileReferenceTypeTest()
    {
        IReference fileReference = new FileReference("NatashaPad.dll");
        Assert.Equal(ReferenceResolver.ReferenceType.LocalFile, fileReference.ReferenceType);
    }

    [Fact]
    public void NuGetReferenceTypeTest()
    {
        IReference fileReference = new NuGetReference("NatashaPad.Core", "1.0.0");
        Assert.Equal(ReferenceResolver.ReferenceType.NuGetPackage, fileReference.ReferenceType);
    }

    [Fact]
    public void FileReferenceEqualsTest()
    {
        var reference1 = new FileReference("NatashaPad.dll");
        var reference2 = new FileReference("NatashaPad.dll");
        Assert.Equal(reference1, reference2);
    }

    [Fact]
    public void NuGetReferenceEqualsTest()
    {
        var reference1 = new NuGetReference("NatashaPad.Core", "1.0.0");
        var reference2 = new NuGetReference("NatashaPad.Core", "1.0.0");
        Assert.Equal(reference1, reference2);
    }
}
