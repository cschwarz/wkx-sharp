using System.Collections.Generic;
using Xunit;

namespace Wkx.Tests
{
    public class MultiLineStringTest
    {
        public static readonly MultiLineString TestMultiLineString = new MultiLineString(new List<LineString>()
        {
            new LineString(new List<Point>() {new Point(16, 48), new Point(18, 48) }),
            new LineString(new List<Point>() {new Point(16, 50), new Point(18, 50) })
        });

        [Fact]
        public void GetCenter()
        {
            Assert.Equal(new Point(17, 49), TestMultiLineString.GetCenter());
        }

        [Fact]
        public void GetBoundingBox()
        {
            Assert.Equal(new BoundingBox(16, 48, 18, 50), TestMultiLineString.GetBoundingBox());
        }
    }
}
