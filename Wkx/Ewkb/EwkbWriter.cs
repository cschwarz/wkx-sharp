namespace Wkx
{
    internal class EwkbWriter : WkbWriter
    {
        protected override void WriteWkbType(GeometryType geometryType, Dimension dimension, int? srid)
        {
            uint dimensionType = 0;

            switch (dimension)
            {
                case Dimension.Xyz: dimensionType = EwkbFlags.HasZ; break;
                case Dimension.Xym: dimensionType = EwkbFlags.HasM; break;
                case Dimension.Xyzm: dimensionType = EwkbFlags.HasZ | EwkbFlags.HasM; break;
            }

            if (srid.HasValue)
            {
                wkbWriter.Write(EwkbFlags.HasSrid + dimensionType + (uint)geometryType);
                wkbWriter.Write(srid.Value);
            }
            else
            {
                wkbWriter.Write(dimensionType + (uint)geometryType);
            }
        }
    }
}
