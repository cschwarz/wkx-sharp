using System;
using System.IO;
using System.Text;

namespace Wkx
{
    public abstract class Geometry
    {
        public abstract GeometryType GeometryType { get; }
        public abstract bool IsEmpty { get; }

        public int? Srid { get; set; }
        public Dimension Dimension { get; set; }

        public abstract Point GetCenter();
        public abstract BoundingBox GetBoundingBox();
        public abstract Geometry CurveToLine(double tolerance);

        public static Geometry Deserialize<T>(string value) where T : IGeometrySerializer
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                return Deserialize<T>(stream);
        }

        public static Geometry Deserialize<T>(byte[] value) where T : IGeometrySerializer
        {
            using (MemoryStream stream = new MemoryStream(value))
                return Deserialize<T>(stream);
        }

        public static Geometry Deserialize<T>(Stream stream) where T : IGeometrySerializer
        {
            return Activator.CreateInstance<T>().Deserialize(stream);
        }

        public void Serialize<T>(Stream stream) where T : IGeometrySerializer
        {
            Activator.CreateInstance<T>().Serialize(this, stream);
        }

        public string SerializeString<T>() where T : IGeometrySerializer
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serialize<T>(memoryStream);
                byte[] data = memoryStream.ToArray();
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
        }

        public byte[] SerializeByteArray<T>() where T : IGeometrySerializer
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serialize<T>(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
