using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiCurve : Geometry, IEquatable<MultiCurve>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiCurve; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Geometry> Geometries { get; private set; }

        public MultiCurve()
        {
            Geometries = new List<Geometry>();
        }

        public MultiCurve(IEnumerable<Geometry> geometries)
        {
            Geometries = new List<Geometry>(geometries);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MultiCurve))
                return false;

            return Equals((MultiCurve)obj);
        }

        public bool Equals(MultiCurve other)
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
