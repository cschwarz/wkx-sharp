using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public abstract class Curve : Geometry, IEquatable<Curve>
    {
        public override GeometryType GeometryType { get { return GeometryType.Curve; } }
        public override bool IsEmpty { get { return !Points.Any(); } }

        public List<Point> Points { get; private set; }

        public Curve()
            : this(new List<Point>())
        {
        }

        public Curve(IEnumerable<Point> points)
        {
            Points = new List<Point>(points);

            if (Points.Any())
                Dimension = Points.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Curve))
                return false;

            return Equals((Curve)obj);
        }

        public bool Equals(Curve other)
        {
            return Points.SequenceEqual(other.Points);
        }

        public override int GetHashCode()
        {
            return new { Points }.GetHashCode();
        }

        public override Point GetCenter()
        {
            return Points.GetCenter();
        }

        public override BoundingBox GetBoundingBox()
        {
            return Points.GetBoundingBox();
        }
    }
}
