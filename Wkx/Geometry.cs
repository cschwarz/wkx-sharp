using System.IO;

namespace Wkx
{
    public abstract class Geometry
    {
        public abstract GeometryType GeometryType { get; }
        public abstract bool IsEmpty { get; }

        public int? Srid { get; set; }
        public Dimension Dimension { get; set; }

        public static Geometry Parse(string value)
        {
            if (value.StartsWith("SRID="))
                return new EwktReader(value).Read();

            return new WktReader(value).Read();
        }

        public static Geometry Parse(byte[] buffer)
        {
            using (MemoryStream memoryStream = new MemoryStream(buffer))
                return Parse(memoryStream);
        }

        public static Geometry Parse(Stream stream)
        {
            bool isEwkb = false;

            using (EndianBinaryReader binaryReader = new EndianBinaryReader(stream))
            {
                binaryReader.IsBigEndian = !binaryReader.ReadBoolean();
                isEwkb = (binaryReader.ReadUInt32() & EwkbFlags.HasSrid) == EwkbFlags.HasSrid;

                stream.Position = 0;

                if (isEwkb)
                    return new EwkbReader(stream).Read();

                return new WkbReader(stream).Read();
            }
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

        public byte[] ToEwkb()
        {
            return new EwkbWriter().Write(this);
        }
    }
}
