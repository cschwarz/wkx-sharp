using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CircularString : Curve, IEquatable<CircularString>
    {
        public override GeometryType GeometryType { get { return GeometryType.CircularString; } }
        public override bool IsEmpty { get { return !Points.Any(); } }

        public List<Point> Points { get; private set; }

        public CircularString()
            : this(new List<Point>())
        {
        }

        public CircularString(IEnumerable<Point> points)
        {
            Points = new List<Point>(points);

            if (Points.Any())
                Dimension = Points.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is CircularString))
                return false;

            return Equals((CircularString)obj);
        }

        public bool Equals(CircularString other)
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
