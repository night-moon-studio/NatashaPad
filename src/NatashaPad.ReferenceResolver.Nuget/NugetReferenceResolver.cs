// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad.ReferenceResolver.Nuget;

public sealed class NuGetReferenceResolver : IReferenceResolver
{
    public string ReferenceType => "nuget";

    public Task<IList<PortableExecutableReference>> Resolve(string reference, CancellationToken cancellationToken = default)
    {
        var packageInfo =
            reference.IndexOf("nuget:", StringComparison.OrdinalIgnoreCase) > -1
                ? reference.Split(':')[1].Split(',')
                : reference.Split(',');
        return NugetHelper.ResolveAssemblies(packageInfo[0].Trim(), packageInfo[1].Trim(), cancellationToken);
    }
}
