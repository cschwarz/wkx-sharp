using System;

namespace Wkx
{
    public class BoundingBox : IEquatable<BoundingBox>
    {
        public Dimension Dimension => ZMin.HasValue ? Dimension.Xyz : Dimension.Xy;

        public double XMin { get; private set; }
        public double YMin { get; private set; }
        public double? ZMin { get; private set; }
        public double XMax { get; private set; }
        public double YMax { get; private set; }
        public double? ZMax { get; private set; }

        public BoundingBox()
        {
        }

        public BoundingBox(double xMin, double yMin, double? zMin, double xMax, double yMax, double? zMax)
        {
            if (zMin.HasValue != zMax.HasValue)
            {
                throw new ArgumentNullException(zMin.HasValue ? nameof(zMax) : nameof(zMin));
            }

            XMin = xMin;
            YMin = yMin;
            ZMin = zMin;
            XMax = xMax;
            YMax = yMax;
            ZMax = zMax;
        }

        public BoundingBox(double xMin, double yMin, double xMax, double yMax)
            :this(xMin, yMin, null, xMax, yMax, null)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is BoundingBox))
                return false;

            return Equals((BoundingBox)obj);
        }

        public bool Equals(BoundingBox other)
        {
            return XMin == other.XMin
                && YMin == other.YMin
                && ZMin == other.ZMin
                && XMax == other.XMax
                && YMax == other.YMax
                && ZMax == other.ZMax;
        }

        public override int GetHashCode()
        {
            return new { XMin, YMin, ZMin, XMax, YMax, ZMax }.GetHashCode();
        }
    }
}
