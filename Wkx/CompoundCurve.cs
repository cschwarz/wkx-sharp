using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CompoundCurve : Geometry, IEquatable<CompoundCurve>
    {
        public override GeometryType GeometryType { get { return GeometryType.CompoundCurve; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Geometry> Geometries { get; private set; }

        public CompoundCurve()
        {
            Geometries = new List<Geometry>();
        }

        public CompoundCurve(IEnumerable<Geometry> geometries)
        {
            Geometries = new List<Geometry>(geometries);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is CompoundCurve))
                return false;

            return Equals((CompoundCurve)obj);
        }

        public bool Equals(CompoundCurve other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }

        public override int GetHashCode()
        {
            return new { Geometries }.GetHashCode();
        }
    }
}
