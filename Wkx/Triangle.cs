using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class Triangle : Polygon, IEquatable<Triangle>
    {
        public override GeometryType GeometryType { get { return GeometryType.Triangle; } }

        public Triangle()
            : base()
        {
        }

        public Triangle(IEnumerable<Point> exteriorRing)
            : base(exteriorRing)
        {
        }

        public Triangle(IEnumerable<Point> exteriorRing, IEnumerable<List<Point>> interiorRings)
            : base(exteriorRing, interiorRings)
        {
        }

        public bool Equals(Triangle other)
        {
            return ExteriorRing.SequenceEqual(other.ExteriorRing) && InteriorRings.SequenceEqual(other.InteriorRings);
        }
    }
}
