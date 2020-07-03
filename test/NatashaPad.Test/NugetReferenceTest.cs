using System.Threading.Tasks;
using Xunit;

namespace NatashaPad.Test
{
    public class NugetReferenceTest
    {
        [Fact]
        public async Task MainTest()
        {
            var resolver = new ReferenceResolver.Nuget.NugetReferenceResolver("WeihanLi.Npoi", "1.9.3");
            var result = await resolver.Resolve();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.False(string.IsNullOrEmpty(result[0].FilePath));
        }
    }
}