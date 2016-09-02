using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class GeometryCollection : Geometry, IEquatable<GeometryCollection>
    {
        public override GeometryType GeometryType { get { return GeometryType.GeometryCollection; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Geometry> Geometries { get; private set; }

        public GeometryCollection()
        {
            Geometries = new List<Geometry>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is GeometryCollection))
                return false;

            return Equals((GeometryCollection)obj);
        }

        public bool Equals(GeometryCollection other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }

        public override int GetHashCode()
        {
            return new { Geometries }.GetHashCode();
        }
    }
}
