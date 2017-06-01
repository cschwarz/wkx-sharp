using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiSurface : Geometry, IEquatable<MultiSurface>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiSurface; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Geometry> Geometries { get; private set; }

        public MultiSurface()
        {
            Geometries = new List<Geometry>();
        }

        public MultiSurface(IEnumerable<Geometry> geometries)
        {
            Geometries = new List<Geometry>(geometries);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MultiSurface))
                return false;

            return Equals((MultiSurface)obj);
        }

        public bool Equals(MultiSurface other)
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
