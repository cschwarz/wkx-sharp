namespace Wkx
{
    internal class EwktWriter : WktWriter
    {
        internal override string Write(Geometry geometry)
        {
            return string.Concat("SRID=", geometry.Srid, ";", base.Write(geometry));
        }
    }
}
