namespace Wkx.Tests
{
    using Xunit;

    public static class PolyhedralSurfaceTests
    {
        public class GetBoundingBoxTests
        {
            [Fact]
            public void BoundingBox_Calculated()
            {
                var target = new PolyhedralSurface(
                    new[] {
                        new Polygon(new[] { new Point(0,0,0), new Point(1,1,0), new Point(0,2,0) }),
                        new Polygon(new[] { new Point(0,0,0), new Point(1,1,0), new Point(1,1,1) }),
                        new Polygon(new[] { new Point(1,1,0), new Point(1,1,1), new Point(0,2,0) }),
                        new Polygon(new[] { new Point(0,2,0), new Point(1,1,1), new Point(0,0,0) })
                    });

                var expected = new BoundingBox(0, 0, 0, 1, 2, 1);
                var actual = target.GetBoundingBox();

                Assert.Equal(expected, actual);
            }
        }
    }
}
