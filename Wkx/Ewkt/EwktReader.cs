using System.Globalization;
using System.Text.RegularExpressions;

namespace Wkx
{
    internal class EwktReader : WktReader
    {
        protected Dimension? geometryDimension;

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

            geometry.Dimension = geometryDimension.HasValue ? geometryDimension.Value : geometry.Dimension;

            return geometry;
        }

        protected override Geometry ReadType(GeometryType defaultType, Dimension defaultDimension, params GeometryType[] types)
        {
            Geometry geometry = base.ReadType(defaultType, defaultDimension, types);

            geometry.Dimension = geometryDimension.HasValue ? geometryDimension.Value : geometry.Dimension;

            return geometry;
        }

        protected override Point MatchCoordinate(Dimension dimension)
        {
            Match match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)");

            if (match.Success)
            {
                geometryDimension = Dimension.Xyzm;
                return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
            }

            match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)");

            if (match.Success)
            {
                if (dimension == Dimension.Xym)
                {
                    geometryDimension = Dimension.Xym;
                    return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), null, double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                }
                else
                {
                    geometryDimension = Dimension.Xyz;
                    return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                }
            }

            geometryDimension = Dimension.Xy;
            match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)");
            return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
        }
    }
}
