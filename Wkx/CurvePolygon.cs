using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CurvePolygon : Surface, IEquatable<CurvePolygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.CurvePolygon; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Curve> Geometries { get; private set; }

        public CurvePolygon()
            : this(new List<Curve>())
        {
        }

        public CurvePolygon(IEnumerable<Curve> geometries)
        {
            Geometries = new List<Curve>(geometries);

            if (Geometries.Any())
                Dimension = Geometries.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is CurvePolygon))
                return false;

            return Equals((CurvePolygon)obj);
        }

        public bool Equals(CurvePolygon other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }

        public override int GetHashCode()
        {
            return new { Geometries }.GetHashCode();
        }

        public override Point GetCenter()
        {
            return Geometries.Select(g => g.GetCenter()).GetCenter();
        }

        public override BoundingBox GetBoundingBox()
        {
            return Geometries.Select(g => g.GetBoundingBox()).GetBoundingBox();
        }
    }
}
