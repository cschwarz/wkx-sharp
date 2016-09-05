using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiPolygon : Geometry, IEquatable<MultiPolygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiPolygon; } }
        public override bool IsEmpty { get { return !Polygons.Any(); } }

        public List<Polygon> Polygons { get; private set; }

        public MultiPolygon()
            : this(new List<Polygon>())
        {
        }

        public MultiPolygon(IEnumerable<Polygon> polygons)
        {
            Polygons = new List<Polygon>(polygons);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MultiPolygon))
                return false;

            return Equals((MultiPolygon)obj);
        }

        public bool Equals(MultiPolygon other)
        {
            return Polygons.SequenceEqual(other.Polygons);
        }

        public override int GetHashCode()
        {
            return new { Polygons }.GetHashCode();
        }
    }
}
