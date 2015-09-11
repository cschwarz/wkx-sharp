using Xunit;

namespace Wkx.Tests
{
    public class ZigZagTest
    {
        [Fact]
        public void Encode()
        {
            Assert.Equal(1, ZigZag.Encode(-1));
            Assert.Equal(2, ZigZag.Encode(1));
            Assert.Equal(3, ZigZag.Encode(-2));
            Assert.Equal(4, ZigZag.Encode(2));
        }

        [Fact]
        public void Dencode()
        {
            Assert.Equal(-1, ZigZag.Decode(1));
            Assert.Equal(1, ZigZag.Decode(2));
            Assert.Equal(-2, ZigZag.Decode(3));
            Assert.Equal(2, ZigZag.Decode(4));
        }
    }
}
