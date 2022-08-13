// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using NatashaPad.ReferenceResolver.Nuget;
using WeihanLi.Extensions;

namespace NatashaPad.Test;

public class ScriptEngineTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly INScriptEngine _scriptEngine;

    public ScriptEngineTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _scriptEngine = new CSharpScriptEngine(new IReferenceResolver[]
        {
            new FileReferenceResolver(),
            new NuGetReferenceResolver()
        });
    }

    [Fact]
    public async Task ExecuteTest()
    {
        DumpOutHelper.OutputAction += Output;
        try
        {
            await _scriptEngine.Execute("\"Hello NatashaPad\".Dump();", new NScriptOptions());

            await _scriptEngine.Execute("Console.WriteLine(\"Hello NatashaPad\");", new NScriptOptions());
        }
        catch (Natasha.Error.NatashaException ex)
        {
            _testOutputHelper.WriteLine(ex.Diagnostics.Select(d => d.ToString()).StringJoin(Environment.NewLine));
            throw;
        }
        finally
        {
            DumpOutHelper.OutputAction -= Output;
        }
    }

    [Fact]
    public async Task ExecuteTestWithReference()
    {
        DumpOutHelper.OutputAction += Output;
        try
        {
            var options = new NScriptOptions();
            options.References.Add(new NuGetReference("WeihanLi.Npoi", "1.9.4"));
            await _scriptEngine.Execute("(1+1).Dump();", options);
        }
        catch (Natasha.Error.NatashaException ex)
        {
            _testOutputHelper.WriteLine(ex.Diagnostics.Select(d => d.ToString()).StringJoin(Environment.NewLine));
            throw;
        }
        finally
        {
            DumpOutHelper.OutputAction -= Output;
        }
    }

    private void Output(string msg)
    {
        _testOutputHelper.WriteLine(msg);
    }

    [Fact]
    public async Task EvalTest()
    {
        var result = await _scriptEngine.Eval("1+1", new NScriptOptions());
        Assert.Equal(2, result);

        result = await _scriptEngine.Eval("\"Hello \" + \"NatashaPad\"", new NScriptOptions());
        Assert.Equal("Hello NatashaPad", result);

        result = await _scriptEngine.Eval("DateTime.Today.ToString(\"yyyyMMdd\")", new NScriptOptions());
        Assert.Equal(DateTime.Today.ToString("yyyyMMdd"), result);
    }

    [Fact]
    public async Task EvalTestWithReference()
    {
        var options = new NScriptOptions();
        options.References.Add(new NuGetReference("WeihanLi.Npoi", "1.9.4"));
        var result = await _scriptEngine.Eval("1+1", options);
        Assert.Equal(2, result);
    }
}
