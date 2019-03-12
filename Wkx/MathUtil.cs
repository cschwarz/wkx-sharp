using System;
using System.Collections.Generic;

namespace Wkx
{
    internal static class MathUtil
    {
        internal static IEnumerable<Point> LinearizeArc(Point startPoint, Point anyPointOnArc, Point endPoint, double tolerance)
        {
            double radius;
            Point origin = GetOriginPoint(startPoint, anyPointOnArc, endPoint, tolerance, out radius);
            if (origin == null)
                return new List<Point>() { startPoint, endPoint };

            double sweepAngle = CalculateSweepAngle(startPoint, anyPointOnArc, endPoint);

            return LinearizeArc(startPoint, origin, endPoint, radius, Math.Abs(sweepAngle) > 180);
        }

        internal static IEnumerable<Point> LinearizeArc(Point startPoint, Point originPoint, Point endPoint, double radius, bool isLargeArc)
        {
            radius = Math.Abs(radius);

            var points = new List<Point>();

            int numSegments = (int)(5.0 * Math.Max(1.0, Math.Sqrt(radius)));

            double startAngle = Math.Atan2(startPoint.Y.Value - originPoint.Y.Value, startPoint.X.Value - originPoint.X.Value);
            double endAngle = Math.Atan2(endPoint.Y.Value - originPoint.Y.Value, endPoint.X.Value - originPoint.X.Value);
            double arcAngle = endAngle - startAngle;

            if (Math.Abs(arcAngle) > Math.PI)
                arcAngle -= 2 * Math.PI * Math.Sign(arcAngle);

            if (isLargeArc)
                arcAngle = -(2 * Math.PI * Math.Sign(arcAngle) - arcAngle);

            double theta = arcAngle / (numSegments - 1);
            double tangentialFactor = Math.Tan(theta);
            double radialFactor = Math.Cos(theta);

            double x = radius * Math.Cos(startAngle);
            double y = radius * Math.Sin(startAngle);

            for (int i = 0; i < numSegments; i++)
            {
                points.Add(new Point(x + originPoint.X.Value, y + originPoint.Y.Value, endPoint.Z));

                double tx = -y;
                double ty = x;

                x += tx * tangentialFactor;
                y += ty * tangentialFactor;

                x *= radialFactor;
                y *= radialFactor;
            }

            points[points.Count - 1] = endPoint;

            return points;
        }

        internal static Point GetOriginPoint(Point startPoint, Point anyPointOnArc, Point endPoint, double tolerance, out double radius)
        {
            radius = 0.0;

            double A = anyPointOnArc.X.Value - startPoint.X.Value;
            double B = anyPointOnArc.Y.Value - startPoint.Y.Value;
            double C = endPoint.X.Value - startPoint.X.Value;
            double D = endPoint.Y.Value - startPoint.Y.Value;

            double E = A * (startPoint.X.Value + anyPointOnArc.X.Value) + B * (startPoint.Y.Value + anyPointOnArc.Y.Value);
            double F = C * (startPoint.X.Value + endPoint.X.Value) + D * (startPoint.Y.Value + endPoint.Y.Value);

            double G = 2.0 * (A * (endPoint.Y.Value - anyPointOnArc.Y.Value) - B * (endPoint.X.Value - anyPointOnArc.X.Value));
            // If G is zero then the three points are collinear and no finite-radius
            // circle through them exists.  Otherwise, the radius of the circle is:
            if (Math.Abs(G) < tolerance)
                return null;

            Point center = new Point((D * E - B * F) / G, (A * F - C * E) / G);
            radius = Math.Sqrt(Math.Pow(startPoint.X.Value - center.X.Value, 2) + Math.Pow(startPoint.Y.Value - center.Y.Value, 2));

            return center;
        }

        internal static double CalculateSweepAngle(Point startPoint, Point intermediatePoint, Point endPoint)
        {
            double d = 2 * (startPoint.X.Value - endPoint.X.Value) * (endPoint.Y.Value - intermediatePoint.Y.Value) + 2 * (intermediatePoint.X.Value - endPoint.X.Value) * (startPoint.Y.Value - endPoint.Y.Value);
            double m1 = (Math.Pow(startPoint.X.Value, 2) - Math.Pow(endPoint.X.Value, 2) + Math.Pow(startPoint.Y.Value, 2) - Math.Pow(endPoint.Y.Value, 2));
            double m2 = (Math.Pow(endPoint.X.Value, 2) - Math.Pow(intermediatePoint.X.Value, 2) + Math.Pow(endPoint.Y.Value, 2) - Math.Pow(intermediatePoint.Y.Value, 2));
            double nx = m1 * (endPoint.Y.Value - intermediatePoint.Y.Value) + m2 * (endPoint.Y.Value - startPoint.Y.Value);
            double ny = m1 * (intermediatePoint.X.Value - endPoint.X.Value) + m2 * (startPoint.X.Value - endPoint.X.Value);
            double cx = nx / d;
            double cy = ny / d;
            Point va = new Point(startPoint.X.Value - cx, startPoint.Y.Value - cy);
            Point vb = new Point(intermediatePoint.X.Value - cx, intermediatePoint.Y.Value - cy);
            Point vc = new Point(endPoint.X.Value - cx, endPoint.Y.Value - cy);
            return va.AngleBetween(vb) + vb.AngleBetween(vc);
        }
    }
}
