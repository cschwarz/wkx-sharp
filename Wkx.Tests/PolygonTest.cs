using System.Collections.Generic;
using Xunit;

namespace Wkx.Tests
{
    public class PolygonTest
    {
        public static readonly Polygon TestPolygon = new Polygon(new List<Point>() { new Point(16, 48), new Point(18, 48), new Point(18, 50), new Point(16, 50), new Point(16, 48) });

        [Fact]
        public void GetCenter()
        {
            Assert.Equal(new Point(17, 49), TestPolygon.GetCenter());
        }

        [Fact]
        public void GetBoundingBox()
        {
            Assert.Equal(new BoundingBox(16, 48, 18, 50), TestPolygon.GetBoundingBox());
        }
    }
}
