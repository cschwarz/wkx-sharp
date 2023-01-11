namespace Wkx.Tests
{
    using Xunit;

    public static class MathUtilTests
    {
        public class MinTests
        {
            [Fact]
            public void BothValues_AreNull_ReturnNull()
            {
                Assert.Null(MathUtil.Min(null, null));
            }

            [Fact]
            public void OneValue_IsNull_ReturnNonNullValue()
            {
                Assert.Equal(0, MathUtil.Min(0, null));
                Assert.Equal(0, MathUtil.Min(null, 0));
            }

            [Fact]
            public void NoNullValues_ReturnMinValue()
            {
                Assert.Equal(1, MathUtil.Min(1, 2));
                Assert.Equal(1, MathUtil.Min(2, 1));
            }
        }

        public class MaxTests
        {
            [Fact]
            public void BothValues_AreNull_ReturnNull()
            {
                Assert.Null(MathUtil.Max(null, null));
            }

            [Fact]
            public void OneValue_IsNull_ReturnNonNullValue()
            {
                Assert.Equal(0, MathUtil.Max(0, null));
                Assert.Equal(0, MathUtil.Max(null, 0));
            }

            [Fact]
            public void NoNullValues_ReturnMaxValue()
            {
                Assert.Equal(2, MathUtil.Max(1, 2));
                Assert.Equal(2, MathUtil.Max(2, 1));
            }
        }
    }
}
