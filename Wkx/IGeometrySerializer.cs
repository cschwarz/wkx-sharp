using System.IO;

namespace Wkx
{
    public interface IGeometrySerializer
    {
        Geometry Deserialize(Stream stream);
        void Serialize(Geometry geometry, Stream stream);
    }
}
