namespace Wkx
{
    internal class EwktWriter : WktWriter
    {
        internal override string Write(Geometry geometry)
        {
            return string.Concat("SRID=", geometry.Srid, ";", base.Write(geometry));
        }

        protected override void WriteWktType(GeometryType geometryType, Dimensions dimensions, bool isEmpty)
        {
            wktBuilder.Append(geometryType.ToString().ToUpperInvariant());

            if  (dimensions == Dimensions.XYM)
                wktBuilder.Append("M");

            if (isEmpty)
                wktBuilder.Append(" EMPTY");
        }
    }
}
