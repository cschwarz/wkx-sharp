using System.IO;

namespace Wkx
{
    public abstract class Geometry
    {
        public abstract GeometryType GeometryType { get; }
        public abstract bool IsEmpty { get; }

        public int? Srid { get; set; }        
        public Dimensions Dimensions { get; set; }
        
        public static Geometry Parse(string value)
        {
            if (value.StartsWith("SRID="))
                return new EwktReader(value).Read();

            return new WktReader(value).Read();
        }

        public static Geometry Parse(byte[] buffer)
        {
            using (MemoryStream memoryStream = new MemoryStream(buffer))
                return new WkbReader(memoryStream).Read();
        }

        public static Geometry Parse(Stream stream)
        {
            return new WkbReader(stream).Read();
        }

        public string ToWkt()
        {
            return new WktWriter().Write(this);
        }

        public string ToEwkt()
        {
            return new EwktWriter().Write(this);
        }

        public byte[] ToWkb()
        {
            return new WkbWriter().Write(this);
        }
    }
}
