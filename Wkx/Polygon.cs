using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class Polygon : Surface, IEquatable<Polygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.Polygon; } }
        public override bool IsEmpty { get { return ExteriorRing.IsEmpty; } }
        
        public LinearRing ExteriorRing { get; private set; }
        public List<LinearRing> InteriorRings { get; private set; }

        public Polygon()
            : this(new List<Point>())
        {
        }

        public Polygon(IEnumerable<Point> exteriorRing)
            : this(new LinearRing(exteriorRing))
        {
        }

        public Polygon(LinearRing exteriorRing)
            : this(exteriorRing, new List<LinearRing>())
        {
        }
        
        public Polygon(LinearRing exteriorRing, IEnumerable<LinearRing> interiorRings)
        {
            ExteriorRing = exteriorRing;
            InteriorRings = new List<LinearRing>(interiorRings);

            if (ExteriorRing.Points.Any())
                Dimension = ExteriorRing.Points.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Polygon))
                return false;

            return Equals((Polygon)obj);
        }

        public bool Equals(Polygon other)
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
            return this;
        }
    }
}
