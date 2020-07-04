using NatashaPad.ReferenceResolver.Nuget;
using NuGet.Versioning;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NatashaPad.Test
{
    public class NugetHelperTest
    {
        [Fact]
        public async Task GetPackages()
        {
            var prefix = "WeihanLi";
            var packages = (await NugetHelper.GetPackages(prefix)).ToArray();
            Assert.NotEmpty(packages);
            Assert.Contains("WeihanLi.Common", packages);
        }

        [Fact]
        public async Task GetPackageVersions()
        {
            var packageName = "WeihanLi.Npoi";
            var versions = (await NugetHelper.GetPackageVersions(packageName)).ToArray();
            Assert.NotEmpty(versions);
            Assert.Contains(versions, v => v.ToString().Equals("1.9.3"));
        }

        [Fact]
        public async Task GetPackageDependencies()
        {
            var packageName = "WeihanLi.Npoi";
            var version = "1.9.3";

            var dependencies = await NugetHelper.GetPackageDependencies(packageName, NuGetVersion.Parse(version));
            Assert.NotNull(dependencies);
            Assert.True(dependencies.ContainsKey("WeihanLi.Common"));
        }
    }
}