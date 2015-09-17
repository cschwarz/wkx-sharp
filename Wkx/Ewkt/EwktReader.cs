using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Wkx
{
    internal class EwktReader : WktReader
    {
        protected Dimensions? geometryDimensions;

        internal EwktReader(string value)
            : base(value)
        {
        }

        internal override Geometry Read()
        {
            Match match = MatchRegex(@"^SRID=(\d+);");

            Geometry geometry = base.Read();

            if (match.Success)
                geometry.Srid = int.Parse(match.Groups[1].Value);

            geometry.Dimensions = geometryDimensions.HasValue ? geometryDimensions.Value : Dimensions.XY;

            return geometry;
        }

        protected override Point MatchCoordinate(Dimensions dimensions)
        {
            Match match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)");

            if (match.Success)
            {
                geometryDimensions = Dimensions.XYZM;
                return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
            }

            match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)");

            if (match.Success)
            {
                if (dimensions.HasFlag(Dimensions.M))
                {
                    geometryDimensions = Dimensions.XYM;
                    return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), null, double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                }
                else
                {
                    geometryDimensions = Dimensions.XYZ;
                    return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                }
            }

            geometryDimensions = Dimensions.XY;
            match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)");
            return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));            
        }
    }
}
