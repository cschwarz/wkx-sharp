using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CurvePolygon : Surface, IEquatable<CurvePolygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.CurvePolygon; } }
        public override bool IsEmpty { get { return ExteriorRing.IsEmpty; } }

        public Curve ExteriorRing { get; private set; }
        public List<Curve> InteriorRings { get; private set; }

        public CurvePolygon()
            : this(new List<Point>())
        {
        }

        public CurvePolygon(IEnumerable<Point> exteriorRing)
            : this(new LinearRing(exteriorRing))
        {
        }

        public CurvePolygon(Curve exteriorRing)
            : this(exteriorRing, new List<Curve>())
        {
        }

        public CurvePolygon(Curve exteriorRing, IEnumerable<Curve> interiorRings)
        {
            ExteriorRing = exteriorRing;
            InteriorRings = new List<Curve>(interiorRings);
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
            return ExteriorRing.Equals(other.ExteriorRing) && InteriorRings.SequenceEqual(other.InteriorRings);
        }

        public override int GetHashCode()
        {
            return new { ExteriorRing, InteriorRings }.GetHashCode();
        }

        public override Point GetCenter()
        {
            return ExteriorRing.GetCenter();
        }

        public override BoundingBox GetBoundingBox()
        {
            return ExteriorRing.GetBoundingBox();
        }

        public override Geometry CurveToLine(double tolerance)
        {
            LinearRing exteriorRing = new LinearRing((ExteriorRing.CurveToLine(tolerance) as LineString).Points);
            IEnumerable<LinearRing> linearRings = InteriorRings.Select(r => new LinearRing((r.CurveToLine(tolerance) as LineString).Points));
            return new Polygon(exteriorRing, linearRings);
        }
    }
}
