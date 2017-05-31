using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CircularString : Geometry, IEquatable<CircularString>
    {
        public override GeometryType GeometryType { get { return GeometryType.CircularString; } }
        public override bool IsEmpty { get { return !Points.Any(); } }

        public List<Point> Points { get; private set; }

        public CircularString()
        {
            Points = new List<Point>();
        }

        public CircularString(IEnumerable<Point> points)
        {
            Points = new List<Point>(points);
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
            throw new NotSupportedException();
        }

        public override BoundingBox GetBoundingBox()
        {
            throw new NotSupportedException();
        }
    }
}
