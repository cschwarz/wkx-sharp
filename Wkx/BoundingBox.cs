using System;

namespace Wkx
{
    public class BoundingBox : IEquatable<BoundingBox>
    {
        public double XMin { get; private set; }
        public double YMin { get; private set; }
        public double XMax { get; private set; }
        public double YMax { get; private set; }

        public BoundingBox()
        {
        }

        public BoundingBox(double xMin, double yMin, double xMax, double yMax)
        {
            XMin = xMin;
            YMin = yMin;
            XMax = xMax;
            YMax = yMax;
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
            return XMin == other.XMin && YMin == other.YMin &&
                XMax == other.XMax && YMax == other.YMax;
        }

        public override int GetHashCode()
        {
            return new { XMin, YMin, XMax, YMax }.GetHashCode();
        }
    }
}
