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
            Assert.Equal(testCase.Data.Wkt, Geometry.Parse(testCase.Data.Wkt).ToWkt());
        }
        
        [Theory]
        [MemberData("TestData")]
        public void ParseWkb(TestCase testCase)
        {
            if (string.IsNullOrEmpty(testCase.Data.WkbResult))
                Assert.Equal(testCase.Data.Wkt, Geometry.Parse(testCase.Data.Wkb.ToByteArray()).ToWkt());
            else
                Assert.Equal(testCase.Data.WkbResult, Geometry.Parse(testCase.Data.Wkb.ToByteArray()).ToWkt());
        }

        [Theory]
        [MemberData("TestData")]
        public void ParseWkbXdr(TestCase testCase)
        {
            if (string.IsNullOrEmpty(testCase.Data.WkbResult))
                Assert.Equal(testCase.Data.Wkt, Geometry.Parse(testCase.Data.WkbXdr.ToByteArray()).ToWkt());
            else
                Assert.Equal(testCase.Data.WkbResult, Geometry.Parse(testCase.Data.WkbXdr.ToByteArray()).ToWkt());
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
    }
}