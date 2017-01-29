using Xunit;

namespace Wkx.Tests
{
    public class PointTest
    {
        [Fact]
        public void GetCenter()
        {
            Assert.Equal(new Point(16, 48), new Point(16, 48).GetCenter());
        }

        [Fact]
        public void GetBoundingBox()
        {
            Assert.Equal(new BoundingBox(16, 48, 16, 48), new Point(16, 48).GetBoundingBox());
        }
    }
}
