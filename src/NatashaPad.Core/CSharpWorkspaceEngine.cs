// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Recommendations;
using System.Collections.Immutable;
using System.Reflection;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace NatashaPad;
public sealed class CSharpWorkspaceEngine
{
    private readonly Dictionary<string, IReferenceResolver> _referenceResolvers;
    public CSharpWorkspaceEngine(IEnumerable<IReferenceResolver> referenceResolvers)
    {
        _referenceResolvers = referenceResolvers.ToDictionary(x => x.ReferenceType, x => x, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<CompletionList> GetCompletions(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default)
    {
        var workspace = new AdhocWorkspace(MefHostServices.DefaultHost);
        var project = workspace.AddProject("NatashaPad", LanguageNames.CSharp);
        var assemblyDirectory = Path.GetDirectoryName(Assembly.Load("System.Runtime").Location);
        foreach (var assemblyPath in Directory.GetFiles(assemblyDirectory!, "*.dll"))
        {
            try
            {
                project = project.AddMetadataReference(MetadataReference.CreateFromFile(assemblyPath));
            }
            catch
            {
                //
            }
        }
        // add reference
        if (scriptOptions.References.Count > 0)
        {
            var references = await scriptOptions.References
                .Select(r => _referenceResolvers[r.ReferenceType].Resolve(r.Reference, cancellationToken))
                .WhenAll()
                .ContinueWith(r => r.Result.SelectMany(_ => _).ToArray(), cancellationToken);
            // add reference
            foreach (var reference in references)
            {
                if (!string.IsNullOrEmpty(reference?.FilePath))
                {
                    project = project.AddMetadataReference(MetadataReference.CreateFromFile(reference.FilePath));
                }
            }
        }
        var codeDocumentInfo = DocumentInfo.Create(DocumentId.CreateNewId(project.Id), "Code", loader: new PlainTextLoader(code));
        var doc = workspace.AddDocument(codeDocumentInfo);
        var completionService = CompletionService.GetService(doc);
        Guard.NotNull(completionService);
        var completions = await completionService.GetCompletionsAsync(doc, code.Length, cancellationToken: cancellationToken);
        return completions;
    }

    public async Task<(EmitResult, ImmutableArray<ISymbol>)> Execute(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default)
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("NatashaPad", LanguageNames.CSharp);

        var codeDocument = DocumentInfo.Create(DocumentId.CreateNewId(project.Id), "Code", loader: new PlainTextLoader(code));
        workspace.AddDocument(codeDocument);

        var usingCode = scriptOptions.UsingList.Select(x => $"global using {x}").StringJoin(Environment.NewLine);
        var usingDocument = DocumentInfo.Create(DocumentId.CreateNewId(project.Id), "__GlobalUsing",
            loader: new PlainTextLoader(usingCode));
        workspace.AddDocument(usingDocument);

        var compilation = await project.GetCompilationAsync(cancellationToken);
        Guard.NotNull(compilation);
        await using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream, cancellationToken: cancellationToken);

        var recommendSymbols = await Recommender.GetRecommendedSymbolsAtPositionAsync(project.Documents.First(x => x.Name == "Code"), code.Length, cancellationToken: cancellationToken);
        return (emitResult, recommendSymbols);
    }
}
