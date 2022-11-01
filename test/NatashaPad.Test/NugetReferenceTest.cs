// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Logging.Abstractions;
using ReferenceResolver;

namespace NatashaPad.Test;

public class NugetReferenceTest
{
    [Fact]
    public async Task MainTest()
    {
        
        var resolver = new ReferenceResolver.NuGetReferenceResolver(new NuGetHelper(NullLoggerFactory.Instance));
        var result = await resolver.Resolve("WeihanLi.Npoi, 2.4.2", "net6.0")
            .ContinueWith(r => r.Result.ToArray());
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.False(string.IsNullOrEmpty(result[0]));
    }
}
