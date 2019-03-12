using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class LineString : Curve, IEquatable<LineString>
    {
        public override GeometryType GeometryType { get { return GeometryType.LineString; } }
        public override bool IsEmpty { get { return !Points.Any(); } }

        public List<Point> Points { get; private set; }

        public LineString()
            : this(new List<Point>())
        {
        }

        public LineString(IEnumerable<Point> points)
        {
            Points = new List<Point>(points);

            if (Points.Any())
                Dimension = Points.First().Dimension;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is LineString))
                return false;

            return Equals((LineString)obj);
        }

        public bool Equals(LineString other)
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

        public override Geometry CurveToLine(double tolerance)
        {
            return this;
        }
    }
}
