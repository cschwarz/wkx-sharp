using System.IO;

namespace Wkx
{
    public class EwktSerializer : IGeometrySerializer
    {
        public Geometry Deserialize(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
                return new EwktReader(streamReader.ReadToEnd()).Read();
        }

        public void Serialize(Geometry geometry, Stream stream)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
                streamWriter.Write(new EwktWriter().Write(geometry));
        }
    }
}
