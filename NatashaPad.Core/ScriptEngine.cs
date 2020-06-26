using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Natasha;
using Natasha.CSharp;
using Natasha.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace NatashaPad
{
    public interface INScriptEngine
    {
        Task Execute(string code, NScriptOptions scriptOptions);

        Task<object> Eval(string code, NScriptOptions scriptOptions);
    }

    public class CSharpScriptEngine : INScriptEngine
    {
        public CSharpScriptEngine()
        {
        }

        public async Task Execute(string code, NScriptOptions scriptOptions)
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

            if (scriptOptions.UsingList.Count > 0)
            {
                code = $"{scriptOptions.UsingList.Select(ns => $"using {ns};").StringJoin(Environment.NewLine)}{Environment.NewLine}{code}";
            }

            DomainManagement.RegisterDefault<AssemblyDomain>();
            var assBuilder = new AssemblyCSharpBuilder(GuidIdGenerator.Instance.NewId());

            assBuilder.Add(code, scriptOptions.UsingList);

            // TODO: add references

            assBuilder.CompilerOption(compiler =>
            {
                compiler.AssemblyOutputKind = AssemblyBuildKind.File;
            });

            var assembly = assBuilder.GetAssembly();
            var targetFramework = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

            var entryPoint = assembly.GetType("Program")?.GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (null != entryPoint)
            {
                entryPoint.Invoke(null, entryPoint.GetParameters().Select(p => p.ParameterType.GetDefaultValue()).ToArray());
            }
            else
            {
                throw new ArgumentException("can not find entry point");
            }

            //using (var executor = new DotNetExecutor(assembly.Location, _outputHelper))
            //{
            //    await executor.ExecuteAsync();
            //}
        }

        public async Task<object> Eval(string code, NScriptOptions scriptOptions)
        {
            // https://github.com/dotnet/roslyn/issues/34111
            var defaultReferences =
                new[]
                    {
                        typeof(object).Assembly,
                        typeof(Enumerable).Assembly,
                        Assembly.Load("netstandard"),
                        Assembly.Load("System.Runtime"),
                    }
                    .Select(assembly => assembly.Location)
                    .Distinct()
                    .Select(l => MetadataReference.CreateFromFile(l))
                    .Cast<MetadataReference>()
                    .ToArray();

            var options = ScriptOptions.Default
                .WithLanguageVersion(LanguageVersion.Latest)
                .AddReferences(defaultReferences)
                .WithImports(scriptOptions.UsingList)
                ;

            var referenceResolvers = scriptOptions.ReferenceResolvers
                                     ?? Array.Empty<IReferenceResolver>();
            if (referenceResolvers.Length > 0)
            {
                var references = await referenceResolvers.Select(r => r.Resolve()).WhenAll();
                options = options.WithReferences(references);
            }

            return await CSharpScript.EvaluateAsync(code, options);
        }
    }
}