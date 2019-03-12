namespace Wkx
{
    public abstract class Curve : Geometry
    {
        public override GeometryType GeometryType { get { return GeometryType.Curve; } }
    }
}
