using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CompoundCurve : Curve, IEquatable<CompoundCurve>
    {
        public override GeometryType GeometryType { get { return GeometryType.CompoundCurve; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Curve> Geometries { get; private set; }

        public CompoundCurve()
        {
            Geometries = new List<Curve>();
        }

        public CompoundCurve(IEnumerable<Curve> geometries)
        {
            Geometries = new List<Curve>(geometries);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is CompoundCurve))
                return false;

            return Equals((CompoundCurve)obj);
        }

        public bool Equals(CompoundCurve other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }

        public override int GetHashCode()
        {
            return new { Geometries }.GetHashCode();
        }

        public override Point GetCenter()
        {
            throw new NotSupportedException();
        }

        public override BoundingBox GetBoundingBox()
        {
            throw new NotSupportedException();
        }

        public override Geometry CurveToLine(double tolerance)
        {
            List<Point> points = new List<Point>();

            foreach (Curve curve in Geometries)
                points.AddRange((curve.CurveToLine(tolerance) as LineString).Points);

            return new LineString(points.Distinct());
        }
    }
}
