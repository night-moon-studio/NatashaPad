// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad.Test;

public class NugetReferenceTest
{
    [Fact]
    public async Task MainTest()
    {
        var resolver = new ReferenceResolver.Nuget.NuGetReferenceResolver();
        var result = await resolver.Resolve("nuget:WeihanLi.Npoi, 1.9.3");
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.False(string.IsNullOrEmpty(result[0].FilePath));
    }
}
