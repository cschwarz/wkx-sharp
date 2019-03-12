using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public abstract class GeometryCollection<T> : Geometry, IEquatable<GeometryCollection<T>> where T : Geometry
    {
        public override GeometryType GeometryType { get { return GeometryType.GeometryCollection; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<T> Geometries { get; private set; }

        public GeometryCollection()
            : this(new List<T>())
        {
        }

        public GeometryCollection(IEnumerable<T> geometries)
        {
            Geometries = new List<T>(geometries);

            if (Geometries.Any())
                Dimension = Geometries.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is GeometryCollection<T>))
                return false;

            return Equals((GeometryCollection<T>)obj);
        }

        public bool Equals(GeometryCollection<T> other)
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

        public override Geometry CurveToLine(double tolerance)
        {
            return new GeometryCollection(Geometries.Select(g => g.CurveToLine(tolerance)));
        }
    }

    public class GeometryCollection : GeometryCollection<Geometry>
    {
        public GeometryCollection()
               : base()
        {
        }

        public GeometryCollection(IEnumerable<Geometry> geometries)
            : base(geometries)
        {
        }
    }
}
