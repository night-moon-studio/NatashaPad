// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using ReferenceResolver;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace NatashaPad.Test;

public class ScriptEngineTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly INScriptEngine _scriptEngine;

    public ScriptEngineTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _scriptEngine = new CSharpScriptEngine(new ReferenceResolverFactory(DependencyResolver.Current));
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
            options.References.Add(new NuGetReference("WeihanLi.Npoi", "2.4.2"));
            options.UsingList.Add("WeihanLi.Npoi");
            await _scriptEngine.Execute("CsvHelper.GetCsvText(new[]{1,2,3}).Dump();", options);
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
        options.References.Add(new NuGetReference("WeihanLi.Npoi", "2.4.2"));
        options.UsingList.Add("WeihanLi.Npoi");
        var result = await _scriptEngine.Eval("CsvHelper.GetCsvText(Enumerable.Range(1, 3))", options);
        Assert.NotNull(result);
    }
}
