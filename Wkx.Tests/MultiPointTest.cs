using System.Collections.Generic;
using Xunit;

namespace Wkx.Tests
{
    public class MultiPointTest
    {
        public static readonly MultiPoint TestMultiPoint = new MultiPoint(new List<Point>() { new Point(16, 48), new Point(18, 50) });

        [Fact]
        public void GetCenter()
        {
            Assert.Equal(new Point(17, 49), TestMultiPoint.GetCenter());
        }

        [Fact]
        public void GetBoundingBox()
        {
            Assert.Equal(new BoundingBox(16, 48, 18, 50), TestMultiPoint.GetBoundingBox());
        }
    }
}
