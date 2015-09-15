using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Xunit;

namespace Wkx.Tests
{
    public class WkxTest
    {
        public static TheoryData<TestCase> TestData;

        static WkxTest()
        {
            TestData = new TheoryData<TestCase>();

            JObject testdata = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("testdata.json"));

            foreach (var test in testdata)
                TestData.Add(new TestCase(test.Key, test.Value.ToObject<TestCaseData>()));
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseWkt(TestCase testCase)
        {
            ParseTest(testCase, t => t.Wkt, false);
        }
        
        [Theory]
        [MemberData("TestData")]
        public void ParseWkb(TestCase testCase)
        {
            ParseTest(testCase, t => t.Wkb, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseWkbXdr(TestCase testCase)
        {
            ParseTest(testCase, t => t.WkbXdr, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ToWkt(TestCase testCase)
        {
            Assert.Equal(testCase.Data.Wkt, Geometry.Parse(testCase.Data.Wkt).ToWkt());
        }

        [Theory]
        [MemberData("TestData")]
        public void ToWkb(TestCase testCase)
        {
            Assert.Equal(testCase.Data.Wkb.ToByteArray(), Geometry.Parse(testCase.Data.Wkt).ToWkb());
        }

        private static void ParseTest(TestCase testCase, Func<TestCaseData, string> testProperty, bool isBinary)
        {
            string wktResult = testCase.Data.Results != null ? testProperty(testCase.Data.Results) : testCase.Data.Wkt;

            if (string.IsNullOrEmpty(wktResult))
                wktResult = testCase.Data.Wkt;

            if (isBinary)
                Assert.Equal(wktResult, Geometry.Parse(testProperty(testCase.Data).ToByteArray()).ToWkt());
            else
                Assert.Equal(wktResult, Geometry.Parse(testProperty(testCase.Data)).ToWkt());
        }
    }
}