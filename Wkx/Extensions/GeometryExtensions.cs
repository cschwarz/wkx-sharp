using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    internal static class GeometryExtensions
    {
        internal static Point GetCenter(this IEnumerable<Point> points)
        {
            double x = 0;
            double y = 0;
            double z = 0;
            double m = 0;
            bool hasX = false;
            bool hasY = false;
            bool hasZ = false;
            bool hasM = false;

            foreach (Point point in points)
            {
                if (point.X.HasValue)
                {
                    x += point.X.Value;
                    hasX = true;
                }
                if (point.Y.HasValue)
                {
                    y += point.Y.Value;
                    hasY = true;
                }
                if (point.Z.HasValue)
                {
                    z += point.Z.Value;
                    hasZ = true;
                }
                if (point.M.HasValue)
                {
                    m += point.M.Value;
                    hasM = true;
                }
            }

            int pointCount = points.Count();

            if (hasX)
                x = x / pointCount;
            if (hasY)
                y = y / pointCount;
            if (hasZ)
                z = z / pointCount;
            if (hasM)
                m = m / pointCount;

            if (hasX && hasY)
                return new Point(x, y, hasZ ? z : (double?)null, hasM ? m : (double?)null);

            return null;
        }

        internal static BoundingBox GetBoundingBox(this IEnumerable<Point> points)
        {
            double xMin = double.MaxValue;
            double yMin = double.MaxValue;
            double xMax = -double.MaxValue;
            double yMax = -double.MaxValue;

            foreach (Point point in points)
            {
                xMin = Math.Min(xMin, point.X.Value);
                yMin = Math.Min(yMin, point.Y.Value);
                xMax = Math.Max(xMax, point.X.Value);
                yMax = Math.Max(yMax, point.Y.Value);
            }

            return new BoundingBox(xMin, yMin, xMax, yMax);
        }

        internal static BoundingBox GetBoundingBox(this IEnumerable<BoundingBox> boundingBoxes)
        {
            double xMin = double.MaxValue;
            double yMin = double.MaxValue;
            double xMax = -double.MaxValue;
            double yMax = -double.MaxValue;

            foreach (BoundingBox boundingBox in boundingBoxes)
            {
                xMin = Math.Min(xMin, boundingBox.XMin);
                yMin = Math.Min(yMin, boundingBox.YMin);
                xMax = Math.Max(xMax, boundingBox.XMax);
                yMax = Math.Max(yMax, boundingBox.YMax);
            }

            return new BoundingBox(xMin, yMin, xMax, yMax);
        }

        internal static double AngleBetween(this Point point, Point otherPoint)
        {
            double x = point.X.Value * otherPoint.X.Value + point.Y.Value * otherPoint.Y.Value;
            double y = point.X.Value * otherPoint.Y.Value - otherPoint.X.Value * point.Y.Value;
            return Math.Atan2(y, x) * (180.0 / Math.PI);
        }
    }
}
