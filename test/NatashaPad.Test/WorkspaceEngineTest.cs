// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using NatashaPad.ReferenceResolver.Nuget;

namespace NatashaPad.Test;
public class WorkspaceEngineTest
{
    [Fact]
    public async Task CompletionTest()
    {
        var code = "Console.";
        var engine = new CSharpWorkspaceEngine(new[] { new NuGetReferenceResolver() });
        var completions = await engine.GetCompletions(code, new NScriptOptions(), CancellationToken.None);
        Assert.NotNull(completions);
    }
}
