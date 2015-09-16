using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Wkx.Tests
{
    public class TestCase : IXunitSerializable
    {
        public string Dimension { get; set; }
        public string Key { get; set; }
        public TestCaseData Data { get; set; }

        public TestCase()
        {
        }

        public TestCase(string dimension, string key, TestCaseData data)
        {
            Dimension = dimension;
            Key = key;
            Data = data;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Dimension = info.GetValue<string>("Dimension");
            Key = info.GetValue<string>("Key");
            Data = JsonConvert.DeserializeObject<TestCaseData>(info.GetValue<string>("Data"));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Dimension", Dimension);
            info.AddValue("Key", Key);
            info.AddValue("Data", JsonConvert.SerializeObject(Data));
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Dimension, Key);
        }
    }
}
