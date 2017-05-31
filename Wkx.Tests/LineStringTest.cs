using System.Collections.Generic;
using Xunit;

namespace Wkx.Tests
{
    public class LineStringTest
    {
        public static readonly LineString TestLineString = new LineString(new List<Point>() { new Point(16, 48), new Point(18, 50) });

        [Fact]
        public void GetCenter()
        {
            Assert.Equal(new Point(17, 49), TestLineString.GetCenter());
        }

        [Fact]
        public void GetBoundingBox()
        {
            Assert.Equal(new BoundingBox(16, 48, 18, 50), TestLineString.GetBoundingBox());
        }
    }
}
