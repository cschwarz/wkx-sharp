using System.Collections.Generic;
using Xunit;

namespace Wkx.Tests
{
    public class GeometryCollectionTest
    {
        public static readonly GeometryCollection TestGeometryCollection = new GeometryCollection(new List<Geometry>() { new Point(16, 48), new Point(18, 50) });

        [Fact]
        public void GetCenter()
        {
            Assert.Equal(new Point(17, 49), TestGeometryCollection.GetCenter());
        }

        [Fact]
        public void GetBoundingBox()
        {
            Assert.Equal(new BoundingBox(16, 48, 18, 50), TestGeometryCollection.GetBoundingBox());
        }
    }
}
