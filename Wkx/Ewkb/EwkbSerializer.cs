using System.IO;

namespace Wkx
{
    public class EwkbSerializer : IGeometrySerializer
    {
        public Geometry Deserialize(Stream stream)
        {
            return new EwkbReader(stream).Read();
        }

        public void Serialize(Geometry geometry, Stream stream)
        {
            byte[] buffer = new EwkbWriter().Write(geometry);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
