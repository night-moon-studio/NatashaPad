using Xunit;

namespace NatashaPad.Test
{
    public class DumperTest
    {
        private readonly IDumper _dumper = DefaultDumper.Instance;

        [Theory]
        [InlineData("NatashaPad")]
        public void StringTest(string str)
        {
            Assert.Equal(str, _dumper.Dump(str));
        }
    }
}