using System;
using Xunit;

namespace Wkx.Tests
{
    public class WktTest
    {
        [Fact]
        public void ParseWkt_ValidInput()
        {
            Assert.Equal(new Point(1, 2), Geometry.Deserialize<WktSerializer>("POINT(1 2)"));
            Assert.Equal(new Point(1.2, 3.4), Geometry.Deserialize<WktSerializer>("POINT(1.2 3.4)"));
            Assert.Equal(new Point(1, 3.4), Geometry.Deserialize<WktSerializer>("POINT(1 3.4)"));
            Assert.Equal(new Point(1.2, 3), Geometry.Deserialize<WktSerializer>("POINT(1.2 3)"));

            Assert.Equal(new Point(-1, -2), Geometry.Deserialize<WktSerializer>("POINT(-1 -2)"));
            Assert.Equal(new Point(-1, 2), Geometry.Deserialize<WktSerializer>("POINT(-1 2)"));
            Assert.Equal(new Point(1, -2), Geometry.Deserialize<WktSerializer>("POINT(1 -2)"));

            Assert.Equal(new Point(-1.2, -3.4), Geometry.Deserialize<WktSerializer>("POINT(-1.2 -3.4)"));
            Assert.Equal(new Point(-1.2, 3.4), Geometry.Deserialize<WktSerializer>("POINT(-1.2 3.4)"));
            Assert.Equal(new Point(1.2, -3.4), Geometry.Deserialize<WktSerializer>("POINT(1.2 -3.4)"));
        }

        [Fact]
        public void ParseWkt_InvalidInput()
        {
            Assert.Equal("Expected geometry type", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("TEST")).Message);
            Assert.Equal("Expected group start", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("POINT)")).Message);
            Assert.Equal("Expected group end", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("POINT(1 2")).Message);
            Assert.Equal("Expected coordinates", Assert.Throws<Exception>(() => Geometry.Deserialize<WktSerializer>("POINT(1)")).Message);
        }
    }
}
