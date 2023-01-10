namespace Wkx.Tests
{
    using System;
    using Xunit;

    public static class BoundingBoxTests
    {
        public class ConstructorTests
        {
            [Fact]
            public void InconsistentZValue_ThrowsException()
            {
                Assert.Throws<ArgumentNullException>(() => new BoundingBox(1, 1, null, 2, 2, 2));
                Assert.Throws<ArgumentNullException>(() => new BoundingBox(1, 1, 1, 2, 2, null));
            }
        }

        public class EqualsTests
        {
            [Fact]
            public void BoundingBoxes_WithoutZ_Match()
            {
                var a = new BoundingBox(1, 1, 2, 2);
                var b = new BoundingBox(1, 1, 2, 2);

                Assert.True(a.Equals(b));
            }

            [Fact]
            public void BoundingBoxes_WithZ_Match()
            {
                var a = new BoundingBox(1, 1, 1, 2, 2, 2);
                var b = new BoundingBox(1, 1, 1, 2, 2, 2);

                Assert.True(a.Equals(b));
            }

            [Fact]
            public void BoundingBoxes_ThatAreDifferent_DoNotMatch()
            {
                var a = new BoundingBox(1, 1, 2, 2);
                var b = new BoundingBox(1, 1, 2, 99);

                Assert.False(a.Equals(b));
            }

            [Fact]
            public void BoundingBoxes_WithMismatchZ_DoNotMatch()
            {
                var a = new BoundingBox(1, 1, 1, 2, 2, 2);
                var b = new BoundingBox(1, 1, 2, 2);

                Assert.False(a.Equals(b));
            }
        }

        public class GetHashCodeTests
        {
            [Fact]
            public void BoundingBoxes_WithoutZ_Match()
            {
                var a = new BoundingBox(1, 1, 2, 2);
                var b = new BoundingBox(1, 1, 2, 2);

                Assert.Equal(a.GetHashCode(), b.GetHashCode());
            }

            [Fact]
            public void BoundingBoxes_WithZ_Match()
            {
                var a = new BoundingBox(1, 1, 1, 2, 2, 2);
                var b = new BoundingBox(1, 1, 1, 2, 2, 2);

                Assert.Equal(a.GetHashCode(), b.GetHashCode());
            }

            [Fact]
            public void BoundingBoxes_ThatAreDifferent_DoNotMatch()
            {
                var a = new BoundingBox(1, 1, 2, 2);
                var b = new BoundingBox(1, 1, 2, 99);

                Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
            }

            [Fact]
            public void BoundingBoxes_WithMismatchZ_DoNotMatch()
            {
                var a = new BoundingBox(1, 1, 1, 2, 2, 2);
                var b = new BoundingBox(1, 1, 2, 2);

                Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
            }
        }
    }
}
