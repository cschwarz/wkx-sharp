using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class PolyhedralSurface : Geometry, IEquatable<PolyhedralSurface>
    {
        public override GeometryType GeometryType { get { return GeometryType.PolyhedralSurface; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Geometry> Geometries { get; private set; }

        public PolyhedralSurface()
            : this(new List<Geometry>())
        {
        }

        public PolyhedralSurface(IEnumerable<Geometry> geometries)
        {
            Geometries = new List<Geometry>(geometries);

            if (Geometries.Any())
                Dimension = Geometries.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is PolyhedralSurface))
                return false;

            return Equals((PolyhedralSurface)obj);
        }

        public bool Equals(PolyhedralSurface other)
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
