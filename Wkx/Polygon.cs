using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class Polygon : Geometry, IEquatable<Polygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.Polygon; } }
        public override bool IsEmpty { get { return !ExteriorRing.Any(); } }

        public List<Point> ExteriorRing { get; private set; }
        public List<List<Point>> InteriorRings { get; private set; }

        public Polygon()
            : this(new List<Point>())
        {
        }

        public Polygon(IEnumerable<Point> exteriorRing)
            : this(exteriorRing, new List<List<Point>>())
        {
        }

        public Polygon(IEnumerable<Point> exteriorRing, IEnumerable<List<Point>> interiorRings)
        {
            ExteriorRing = new List<Point>(exteriorRing);
            InteriorRings = new List<List<Point>>(interiorRings);
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
            return ExteriorRing.SequenceEqual(other.ExteriorRing) && InteriorRings.SequenceEqual(other.InteriorRings);
        }

        public override int GetHashCode()
        {
            return new { ExteriorRing, InteriorRings }.GetHashCode();
        }
    }
}
