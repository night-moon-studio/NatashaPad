using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Natasha.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace NatashaPad
{
    public interface INScriptEngine
    {
        Task Execute(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default);

        Task<object> Eval(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default);
    }

    public class CSharpScriptEngine : INScriptEngine
    {
        private static readonly BindingFlags _mainMethodBindingFlags =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

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

        public async Task Execute(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default)
        {
            if (code.Contains("static void Main(") || code.Contains("static async Task Main("))
            {
                // Program
            }
            else
            {
                // Statement
                if (code.Contains("await "))
                {
                    //async
                    code = $@"public static async Task MainAsync()
  {{
    {code}
  }}";
                }
                else
                {
                    // sync
                    code = $@"public static void Main(string[] args)
  {{
    {code}
  }}";
                }
            }

            if (!code.Contains("static void Main(") && code.Contains("static async Task Main("))
            {
                if (code.Contains("static async Task Main()"))
                {
                    code = $@"{code}
public static void Main() => MainAsync().Wait();
";
                }
                else
                {
                    code = $@"{code}
public static void Main() => MainAsync(null).Wait();
";
                }
            }

            if (!code.Contains("class Program"))
            {
                code = $@"public class Program
{{
  {code}
}}";
            }

            scriptOptions.UsingList.Add("NatashaPad");

            code = $"{scriptOptions.UsingList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(ns => $"using {ns};").StringJoin(Environment.NewLine)}{Environment.NewLine}{code}";

            using var domain = DomainManagement.Random;
            var assBuilder = new AssemblyCSharpBuilder();

            assBuilder.Add(code, scriptOptions.UsingList);

            // add reference
            if (scriptOptions.ReferenceResolvers.Count > 0)
            {
                var references = await scriptOptions.ReferenceResolvers
                    .Select(r => r.Resolve())
                    .WhenAll()
                    .ContinueWith(r => r.Result.SelectMany(_ => _).ToArray());
                // add reference
                foreach (var reference in references)
                {
                    // TODO: handle none filePath reference
                    if (!string.IsNullOrEmpty(reference?.FilePath))
                    {
                        domain.LoadPluginFromStream(reference.FilePath);
                    }
                }
            }
            assBuilder.Compiler.Domain = domain;
            assBuilder.Compiler.AssemblyOutputKind = AssemblyBuildKind.File;

            var assembly = assBuilder.GetAssembly();

            using (var capture = await ConsoleOutput.CaptureAsync())
            {
                var entryPoint = assembly.EntryPoint
                    ?? assembly.GetType("Program")?.GetMethod("Main", _mainMethodBindingFlags)
                    ?? assembly.GetType("Program")?.GetMethod("MainAsync", _mainMethodBindingFlags)
                    ;
                if (null != entryPoint)
                {
                    entryPoint.Invoke(null, entryPoint.GetParameters().Select(p => p.ParameterType.GetDefaultValue()).ToArray());
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
        }

        public async Task<object> Eval(string code, NScriptOptions scriptOptions, CancellationToken cancellationToken = default)
        {
            var originalReferences = new[]
            {
                typeof(object).Assembly,
                typeof(Enumerable).Assembly,
                typeof(IDumper).Assembly,
                Assembly.Load("netstandard"),
                Assembly.Load("System.Runtime"),
            };
            // https://github.com/dotnet/roslyn/issues/34111
            var defaultReferences =
                    originalReferences
                    .SelectMany(ass => ass.GetReferencedAssemblies())
                    .Distinct()
                    .Select(Assembly.Load)
                    .Union(originalReferences)
                    .Select(assembly => assembly.Location)
                    .Distinct()
                    .Select(l => MetadataReference.CreateFromFile(l))
                    .Cast<MetadataReference>()
                    .ToArray();

            scriptOptions.UsingList.Add("NatashaPad");
            var options = ScriptOptions.Default
                .WithLanguageVersion(LanguageVersion.Latest)
                .AddReferences(defaultReferences)
                .WithImports(scriptOptions.UsingList.Where(x => !string.IsNullOrWhiteSpace(x)))
                ;

            if (scriptOptions.ReferenceResolvers.Count > 0)
            {
                var references = await scriptOptions.ReferenceResolvers
                        .Select(r => r.Resolve())
                        .WhenAll()
                        .ContinueWith(r => r.Result.SelectMany(_ => _), cancellationToken)
                    ;
                options = options.AddReferences(references);
            }

            return await CSharpScript.EvaluateAsync(code, options, cancellationToken: cancellationToken);
        }
    }
}