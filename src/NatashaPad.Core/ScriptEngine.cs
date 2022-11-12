using System.Runtime.Versioning;
// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Natasha.Framework;
using ReferenceResolver;
using System.Reflection;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace NatashaPad;

public interface INScriptEngine
{
    Task Execute(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default);

    Task<object> Eval(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default);
}

public class CSharpScriptEngine : INScriptEngine
{
    private const BindingFlags MainMethodBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    static CSharpScriptEngine()
    {
        try
        {
            NatashaInitializer.Initialize()
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            InvokeHelper.OnInvokeException?.Invoke(ex);
        }
    }

    private readonly IReferenceResolverFactory _referenceResolverFactory;
    public CSharpScriptEngine(IReferenceResolverFactory referenceResolverFactory)
    {
        _referenceResolverFactory = referenceResolverFactory;
    }

    public async Task Execute(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default)
    {
        scriptOptions.UsingList.Add("NatashaPad");

        code = $"{scriptOptions.UsingList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(ns => $"using {ns};").StringJoin(Environment.NewLine)}{Environment.NewLine}{code}";

        using var domain = DomainManagement.Random;
        var assBuilder = new AssemblyCSharpBuilder();

        assBuilder.Add(code, scriptOptions.UsingList);

        // add reference
        if (scriptOptions.References.Count > 0)
        {
            var references = await scriptOptions.References
                .Select(r => _referenceResolverFactory.GetResolver(r.ReferenceType)
                        .Resolve(r.Reference, scriptOptions.TargetFramework, cancellationToken)
                )
                .WhenAll()
                .ContinueWith(r => r.Result.SelectMany(_ => _).ToArray(), cancellationToken);
            // add reference
            foreach (var reference in references)
            {
                if (!string.IsNullOrEmpty(reference))
                {
                    domain.LoadAssemblyFromStream(reference);
                }
            }
        }

        assBuilder.Compiler.AssemblyKind = OutputKind.ConsoleApplication;
        assBuilder.Compiler.Domain = domain;
        assBuilder.Compiler.AssemblyOutputKind = AssemblyBuildKind.Stream;

        var assembly = assBuilder.GetAssembly();

        using var capture = await ConsoleOutput.CaptureAsync();
        var entryPoint = assembly.EntryPoint
                         ?? assembly.GetType("Program")?.GetMethod("Main", MainMethodBindingFlags)
                         ?? assembly.GetType("Program")?.GetMethod("MainAsync", MainMethodBindingFlags)
            ;
        if (null != entryPoint)
        {
            var returnValue = entryPoint.Invoke(null, entryPoint.GetParameters().Select(p => p.ParameterType.GetDefaultValue()).ToArray());
            await TaskHelper.ToTask(returnValue);
        }
        else
        {
            throw new ArgumentException("can not find entry point");
        }
        if (!string.IsNullOrEmpty(capture.StandardOutput))
        {
            DumpOutHelper.OutputAction?.Invoke(capture.StandardOutput);
        }
        if (!string.IsNullOrEmpty(capture.StandardError))
        {
            DumpOutHelper.OutputAction?.Invoke($"Error:{capture.StandardError}");
        }
    }

    public async Task<object> Eval(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default)
    {
        var defaultReferences =
            await _referenceResolverFactory.GetResolver(ReferenceType.FrameworkReference)
                .ResolveMetadata(FrameworkReferenceResolver.FrameworkNames.Default, scriptOptions.TargetFramework, cancellationToken)
                .ContinueWith(r => r.Result.ToArray());
        scriptOptions.UsingList.Add("NatashaPad");
        var options = ScriptOptions.Default
                .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest)
                .AddReferences(defaultReferences)
                .AddReferences(new[]
                {
                    MetadataReference.CreateFromFile(typeof(DumperResolver).Assembly.Location)
                })
                .WithImports(scriptOptions.UsingList.Where(x => !string.IsNullOrWhiteSpace(x)))
            ;

        if (scriptOptions.References.Count > 0)
        {
            var references = await scriptOptions.References
                    .Select(r => _referenceResolverFactory.ResolveMetadataReference(r.Reference, scriptOptions.TargetFramework, cancellationToken))
                    .WhenAll()
                    .ContinueWith(r => r.Result.SelectMany(_ => _), cancellationToken)
                ;
            options = options.AddReferences(references);
        }

        return await CSharpScript.EvaluateAsync(code, options, cancellationToken: cancellationToken);
    }
}
