// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Logging.Abstractions;
using ReferenceResolver;

namespace NatashaPad.Test;

public class NugetHelperTest
{
    private readonly NuGetHelper _nugetHelper = new(NullLoggerFactory.Instance);
    
    [Fact]
    public async Task GetPackages()
    {
        var prefix = "WeihanLi";
        var packages = (await _nugetHelper.GetPackages(prefix)).ToArray();
        Assert.NotEmpty(packages);
        Assert.Contains("WeihanLi.Common", packages);
    }

    [Fact]
    public async Task GetPackageVersions()
    {
        var packageName = "WeihanLi.Npoi";
        var versions = (await _nugetHelper.GetPackageVersions(packageName)).ToArray();
        Assert.NotEmpty(versions);
        Assert.Contains(versions, v => v.ToString().Equals("1.9.3"));
    }
    
    [Fact]
    public async Task GetPackageDependencies()
    {
        var packageName = "WeihanLi.Npoi";
        var version = "1.9.3";
    
        var dependencies = await _nugetHelper.GetPackageDependencies(packageName, NuGet.Versioning.NuGetVersion.Parse(version), "net6.0");
        Assert.NotNull(dependencies);
        Assert.True(dependencies.ContainsKey("WeihanLi.Common"));
    }
}
