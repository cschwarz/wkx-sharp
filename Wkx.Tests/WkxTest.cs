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

            JObject testData = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("testdata.json"));

            foreach (var dimension in testData)
            {
                foreach (var test in dimension.Value.ToObject<JObject>())
                {
                    TestData.Add(new TestCase(dimension.Key, test.Key, test.Value.ToObject<TestCaseData>()));
                }
            }
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseWkt(TestCase testCase)
        {
            ParseTest(testCase, t => t.Wkt, false);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseEwkt(TestCase testCase)
        {
            ParseTest(testCase, t => t.Ewkt, false);
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
        public void ParseEwkb(TestCase testCase)
        {
            ParseTest(testCase, t => t.Ewkb, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseEwkbXdr(TestCase testCase)
        {
            ParseTest(testCase, t => t.EwkbXdr, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ToWkt(TestCase testCase)
        {
            SerializeTest(testCase, g => g.ToWkt(), t => t.Wkt);
        }

        [Theory]
        [MemberData("TestData")]
        public void ToEwkt(TestCase testCase)
        {
            SerializeTest(testCase, g => g.ToEwkt(), t => t.Ewkt);
        }

        [Theory]
        [MemberData("TestData")]
        public void ToWkb(TestCase testCase)
        {
            SerializeTest(testCase, g => g.ToWkb(), t => t.Wkb.ToByteArray());
        }

        [Theory]
        [MemberData("TestData")]
        public void ToEwkb(TestCase testCase)
        {
            SerializeTest(testCase, g => g.ToEwkb(), t => t.Ewkb.ToByteArray());
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

        private static void SerializeTest<T>(TestCase testCase, Func<Geometry, T> serializeFunction, Func<TestCaseData, T> resultProperty)
        {
            Geometry geometry = Geometry.Parse(testCase.Data.Wkt);
            geometry.Srid = 4326;

            Assert.Equal(resultProperty(testCase.Data), serializeFunction(geometry));
        }        
    }
}