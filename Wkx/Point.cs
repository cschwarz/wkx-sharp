using System;

namespace Wkx
{
    public class Point : Geometry, IEquatable<Point>
    {
        public override GeometryType GeometryType { get { return GeometryType.Point; } }
        public override bool IsEmpty { get { return (!X.HasValue || double.IsNaN(X.Value)) && (!Y.HasValue || double.IsNaN(Y.Value)); } }

        public double? X { get; private set; }
        public double? Y { get; private set; }
        public double? Z { get; private set; }
        public double? M { get; private set; }

        public Point()
        {
        }

        public Point(double x, double y, double? z = null, double? m = null)
        {
            X = x;
            Y = y;
            Z = z;
            M = m;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Point))
                return false;

            return Equals((Point)obj);
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y &&
                Z == other.Z && M == other.M;
        }

        public override int GetHashCode()
        {
            return new { X, Y, Z, M }.GetHashCode();
        }
    }
}