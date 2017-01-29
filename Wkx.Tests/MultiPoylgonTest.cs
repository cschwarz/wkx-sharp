using System.Collections.Generic;
using Xunit;

namespace Wkx.Tests
{
    public class MultiPolygonTest
    {
        public static readonly MultiPolygon TestMultiPolygon = new MultiPolygon(new List<Polygon>()
        {
            new Polygon(new List<Point>() { new Point(16, 48), new Point(18, 48), new Point(18, 50), new Point(16, 50), new Point(16, 48) }),
            new Polygon(new List<Point>() { new Point(16, 50), new Point(18, 50), new Point(18, 52), new Point(16, 52), new Point(16, 50) })
        });

        [Fact]
        public void GetCenter()
        {
            Assert.Equal(new Point(17, 50), TestMultiPolygon.GetCenter());
        }

        [Fact]
        public void GetBoundingBox()
        {
            Assert.Equal(new BoundingBox(16, 48, 18, 52), TestMultiPolygon.GetBoundingBox());
        }
    }
}
