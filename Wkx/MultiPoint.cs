using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiPoint : Geometry, IEquatable<MultiPoint>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiPoint; } }
        public override bool IsEmpty { get { return !Points.Any(); } }

        public List<Point> Points { get; private set; }

        public MultiPoint()
            : this(new List<Point>())
        {
        }

        public MultiPoint(IEnumerable<Point> points)
        {
            Points = new List<Point>(points);

            if (Points.Any())
                Dimension = Points.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MultiPoint))
                return false;

            return Equals((MultiPoint)obj);
        }

        public bool Equals(MultiPoint other)
        {
            return Points.SequenceEqual(other.Points);
        }

        public override int GetHashCode()
        {
            return new { Points }.GetHashCode();
        }

        public override Point GetCenter()
        {
            return Points.Select(p => p.GetCenter()).GetCenter();
        }

        public override BoundingBox GetBoundingBox()
        {
            return Points.Select(p => p.GetBoundingBox()).GetBoundingBox();
        }
    }
}
