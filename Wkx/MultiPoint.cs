using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiPoint : GeometryCollection<Point>, IEquatable<MultiPoint>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiPoint; } }

        public MultiPoint()
            : base(new List<Point>())
        {
        }

        public MultiPoint(IEnumerable<Point> points)
            : base(points)
        {
        }

        public bool Equals(MultiPoint other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }

        public override Geometry CurveToLine(double tolerance)
        {
            return this;
        }
    }
}
