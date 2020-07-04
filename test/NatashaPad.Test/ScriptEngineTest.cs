using System;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace NatashaPad.Test
{
    public class ScriptEngineTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly INScriptEngine _scriptEngine;

        public ScriptEngineTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _scriptEngine = new CSharpScriptEngine();
        }

        [Fact]
        public async Task ExecuteTest()
        {
            DumpOutHelper.OutputAction += Output;
            try
            {
                await _scriptEngine.Execute("\"Hello NatashaPad\".Dump();", new NScriptOptions());
            }
            catch (Natasha.Error.CompilationException ex)
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
    }
}