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
            ParseTest<WktSerializer>(testCase, t => t.Wkt, false);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseEwkt(TestCase testCase)
        {
            ParseTest<EwktSerializer>(testCase, t => t.Ewkt, false);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseWkb(TestCase testCase)
        {
            ParseTest<WkbSerializer>(testCase, t => t.Wkb, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseWkbXdr(TestCase testCase)
        {
            ParseTest<WkbSerializer>(testCase, t => t.WkbXdr, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseEwkb(TestCase testCase)
        {
            ParseTest<EwkbSerializer>(testCase, t => t.Ewkb, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseEwkbXdr(TestCase testCase)
        {
            ParseTest<EwkbSerializer>(testCase, t => t.EwkbXdr, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseEwkbNoSrid(TestCase testCase)
        {
            ParseTest<EwkbSerializer>(testCase, t => t.EwkbNoSrid, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseEwkbXdrNoSrid(TestCase testCase)
        {
            ParseTest<EwkbSerializer>(testCase, t => t.EwkbXdrNoSrid, true);
        }

        [Theory]
        [MemberData("TestData")]
        public void ToWkt(TestCase testCase)
        {
            SerializeTest(testCase, g => g.SerializeString<WktSerializer>(), t => t.Wkt);
        }

        [Theory]
        [MemberData("TestData")]
        public void ToEwkt(TestCase testCase)
        {
            SerializeTest(testCase, g => g.SerializeString<EwktSerializer>(), t => t.Ewkt);
        }

        [Theory]
        [MemberData("TestData")]
        public void ToWkb(TestCase testCase)
        {
            SerializeTest(testCase, g => g.SerializeByteArray<WkbSerializer>(), t => t.Wkb.ToByteArray());
        }

        [Theory]
        [MemberData("TestData")]
        public void ToEwkb(TestCase testCase)
        {
            SerializeTest(testCase, g => g.SerializeByteArray<EwkbSerializer>(), t => t.Ewkb.ToByteArray());
        }

        private static void ParseTest<T>(TestCase testCase, Func<TestCaseData, string> testProperty, bool isBinary)
            where T : IGeometrySerializer
        {
            string wktResult = testCase.Data.Results != null ? testProperty(testCase.Data.Results) : testCase.Data.Wkt;

            if (string.IsNullOrEmpty(wktResult))
                wktResult = testCase.Data.Wkt;

            if (isBinary)
                Assert.Equal(wktResult, Geometry.Deserialize<T>(testProperty(testCase.Data).ToByteArray()).SerializeString<WktSerializer>());
            else
                Assert.Equal(wktResult, Geometry.Deserialize<T>(testProperty(testCase.Data)).SerializeString<WktSerializer>());
        }

        private static void SerializeTest<T>(TestCase testCase, Func<Geometry, T> serializeFunction, Func<TestCaseData, T> resultProperty)
        {
            Geometry geometry = Geometry.Deserialize<WktSerializer>(testCase.Data.Wkt);
            geometry.Srid = 4326;

            Assert.Equal(resultProperty(testCase.Data), serializeFunction(geometry));
        }
    }
}