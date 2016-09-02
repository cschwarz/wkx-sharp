using System.IO;

namespace Wkx
{
    public class WktSerializer : IGeometrySerializer
    {
        public Geometry Deserialize(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
                return new WktReader(streamReader.ReadToEnd()).Read();
        }

        public void Serialize(Geometry geometry, Stream stream)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
                streamWriter.Write(new WktWriter().Write(geometry));
        }
    }
}
