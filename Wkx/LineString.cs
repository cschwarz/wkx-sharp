using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class LineString : Curve, IEquatable<LineString>
    {
        public override GeometryType GeometryType { get { return GeometryType.LineString; } }

        public LineString()
            : base()
        {
        }

        public LineString(IEnumerable<Point> points)
            : base(points)
        {
        }
        
        public bool Equals(LineString other)
        {
            return Points.SequenceEqual(other.Points);
        }        
    }
}
