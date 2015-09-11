using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Wkx.Tests
{
    public class TestCase : IXunitSerializable
    {
        public string Key { get; set; }
        public TestCaseData Data { get; set; }

        public TestCase()
        {
        }

        public TestCase(string key, TestCaseData data)
        {
            Key = key;
            Data = data;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Key = info.GetValue<string>("Key");
            Data = JsonConvert.DeserializeObject<TestCaseData>(info.GetValue<string>("Data"));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Key", Key);
            info.AddValue("Data", JsonConvert.SerializeObject(Data));
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
