using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CurvePolygon : Geometry, IEquatable<CurvePolygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.CurvePolygon; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Geometry> Geometries { get; private set; }

        public CurvePolygon()
        {
            Geometries = new List<Geometry>();
        }

        public CurvePolygon(IEnumerable<Geometry> geometries)
        {
            Geometries = new List<Geometry>(geometries);
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
            throw new NotSupportedException();
        }

        public override BoundingBox GetBoundingBox()
        {
            throw new NotSupportedException();
        }
    }
}
