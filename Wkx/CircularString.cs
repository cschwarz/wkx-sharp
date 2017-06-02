using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CircularString : Curve, IEquatable<CircularString>
    {
        public override GeometryType GeometryType { get { return GeometryType.CircularString; } }
        
        public CircularString()
            : base()
        {
        }

        public CircularString(IEnumerable<Point> points)
            : base(points)
        {
        }
        
        public bool Equals(CircularString other)
        {
            return Points.SequenceEqual(other.Points);
        }        
    }
}
