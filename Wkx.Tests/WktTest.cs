using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;

namespace Wkx.Tests
{
    public class WktTest
    {
        [Fact]
        public void ParseWkt_ValidInput()
        {
            Assert.Equal(new Point(1, 2), Geometry.Deserialize<WktSerializer>("POINT(1 2)"));
            Assert.Equal(new Point(1, 2, 3, 4), Geometry.Deserialize<WktSerializer>("POINT(1 2 3 4)"));
            Assert.Equal(new Point(1, 2, 3), Geometry.Deserialize<WktSerializer>("POINT(1 2 3)"));
            Assert.Equal(new Point(1.2, 3.4), Geometry.Deserialize<WktSerializer>("POINT(1.2 3.4)"));
            Assert.Equal(new Point(1, 3.4), Geometry.Deserialize<WktSerializer>("POINT(1 3.4)"));
            Assert.Equal(new Point(1.2, 3), Geometry.Deserialize<WktSerializer>("POINT(1.2 3)"));

            Assert.Equal(new Point(-1, -2), Geometry.Deserialize<WktSerializer>("POINT(-1 -2)"));
            Assert.Equal(new Point(-1, 2), Geometry.Deserialize<WktSerializer>("POINT(-1 2)"));
            Assert.Equal(new Point(1, -2), Geometry.Deserialize<WktSerializer>("POINT(1 -2)"));

            Assert.Equal(new Point(-1.2, -3.4), Geometry.Deserialize<WktSerializer>("POINT(-1.2 -3.4)"));
            Assert.Equal(new Point(-1.2, 3.4), Geometry.Deserialize<WktSerializer>("POINT(-1.2 3.4)"));
            Assert.Equal(new Point(1.2, -3.4), Geometry.Deserialize<WktSerializer>("POINT(1.2 -3.4)"));

            Assert.Equal(new Point(12, 34), Geometry.Deserialize<WktSerializer>("POINT(1.2e1 3.4e1)"));
            Assert.Equal(new Point(0.12, 0.34), Geometry.Deserialize<WktSerializer>("POINT(1.2e-1 3.4e-1)"));
            Assert.Equal(new Point(-12, -34), Geometry.Deserialize<WktSerializer>("POINT(-1.2e1 -3.4e1)"));
            Assert.Equal(new Point(-0.12, -0.34), Geometry.Deserialize<WktSerializer>("POINT(-1.2e-1 -3.4e-1)"));

            Assert.Equal(new MultiPoint(new List<Point>() { new Point(1, 2), new Point(3, 4) }), Geometry.Deserialize<WktSerializer>("MULTIPOINT(1 2,3 4)"));
            Assert.Equal(new MultiPoint(new List<Point>() { new Point(1, 2), new Point(3, 4) }), Geometry.Deserialize<WktSerializer>("MULTIPOINT(1 2, 3 4)"));
            Assert.Equal(new MultiPoint(new List<Point>() { new Point(1, 2), new Point(3, 4) }), Geometry.Deserialize<WktSerializer>("MULTIPOINT((1 2),(3 4))"));
            Assert.Equal(new MultiPoint(new List<Point>() { new Point(1, 2), new Point(3, 4) }), Geometry.Deserialize<WktSerializer>("MULTIPOINT((1 2), (3 4))"));
        }

        [Fact]
        public void ParseWkt_InvalidInput()
        {
            Assert.Equal("Expected geometry type", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("TEST")).Message);
            Assert.Equal("Expected group start", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("POINT)")).Message);
            Assert.Equal("Expected group end", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("POINT(1 2")).Message);
            Assert.Equal("Expected coordinates", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("POINT(1)")).Message);
        }

        [Fact]
        public void SerializeWkt_InvariantCulture()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Assert.Equal("POINT(1.2 3.4)", new Point(1.2, 3.4).SerializeString<WktSerializer>());
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        [Fact]
        public void SerializeWkt_GermanCulture()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");
            Assert.Equal("POINT(1.2 3.4)", new Point(1.2, 3.4).SerializeString<WktSerializer>());
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        [Fact]
        public void ParseWkt_Performance()
        {
            int pointCount = 50000;
            string wktLineString = string.Concat("LINESTRING(", string.Join(", ", Enumerable.Range(0, pointCount).Select(i => string.Concat(i, " ", i + 1))), ")");
            LineString lineString = Geometry.Deserialize<WktSerializer>(wktLineString) as LineString;
            Assert.Equal(pointCount, lineString.Points.Count);
        }
    }
}
