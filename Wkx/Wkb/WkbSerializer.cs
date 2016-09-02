using System.IO;

namespace Wkx
{
    public class WkbSerializer : IGeometrySerializer
    {
        public Geometry Deserialize(Stream stream)
        {
            return new WkbReader(stream).Read();
        }

        public void Serialize(Geometry geometry, Stream stream)
        {
            byte[] buffer = new WkbWriter().Write(geometry);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
