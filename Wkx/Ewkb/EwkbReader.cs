using System.IO;

namespace Wkx
{
    internal class EwkbReader : WkbReader
    {
        internal EwkbReader(Stream stream)
            : base(stream)
        {
        }

        protected override GeometryType ReadGeometryType(uint type)
        {
            return (GeometryType)(type & 0XFF);
        }

        protected override Dimension ReadDimension(uint type)
        {
            if ((type & EwkbFlags.HasZ) == EwkbFlags.HasZ && (type & EwkbFlags.HasM) == EwkbFlags.HasM)
                return Dimension.Xyzm;
            else if ((type & EwkbFlags.HasZ) == EwkbFlags.HasZ)
                return Dimension.Xyz;
            else if ((type & EwkbFlags.HasM) == EwkbFlags.HasM)
                return Dimension.Xym;

            return Dimension.Xy;
        }

        protected override int? ReadSrid(uint type)
        {
            if ((type & EwkbFlags.HasSrid) == EwkbFlags.HasSrid)
                return wkbReader.ReadInt32();

            return null;
        }
    }
}
