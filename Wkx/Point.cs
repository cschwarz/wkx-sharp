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

            if (z.HasValue && m.HasValue)
                Dimension = Dimension.Xyzm;
            else if (z.HasValue)
                Dimension = Dimension.Xyz;
            else if (m.HasValue)
                Dimension = Dimension.Xym;
            else
                Dimension = Dimension.Xy;
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

        public override Point GetCenter()
        {
            return new Point(X.Value, Y.Value, Z, M);
        }

        public override BoundingBox GetBoundingBox()
        {
            return new BoundingBox(X.Value, Y.Value, X.Value, Y.Value);
        }

        public override Geometry CurveToLine(double tolerance)
        {
            return this;
        }
    }
}