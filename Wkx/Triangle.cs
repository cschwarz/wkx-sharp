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

        public Triangle(LinearRing exteriorRing)
            : base(exteriorRing)
        {
        }

        public Triangle(LinearRing exteriorRing, IEnumerable<LinearRing> interiorRings)
            : base(exteriorRing, interiorRings)
        {
        }

        public bool Equals(Triangle other)
        {
            return ExteriorRing.Equals(other.ExteriorRing) && InteriorRings.SequenceEqual(other.InteriorRings);
        }
    }
}
