using System.IO;
using System.Text;

namespace Wkx.Tests
{
    public static class GeometryExtensions
    {
        public static string SerializeString<T>(this Geometry geometry) where T : IGeometrySerializer
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                geometry.Serialize<T>(memoryStream);
                byte[] data = memoryStream.ToArray();
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
        }

        public static byte[] SerializeByteArray<T>(this Geometry geometry) where T : IGeometrySerializer
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                geometry.Serialize<T>(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
