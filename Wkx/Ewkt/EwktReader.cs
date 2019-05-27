using System;
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
            string coordinateValue = Peek(',', ')');

            if (string.IsNullOrEmpty(coordinateValue))
                throw new Exception("Expected coordinates");

            string[] coordinates = coordinateValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (coordinates.Length)
            {
                case 2: return new Point(double.Parse(coordinates[0], CultureInfo.InvariantCulture), double.Parse(coordinates[1], CultureInfo.InvariantCulture));
                case 3:
                    {
                        if (dimension == Dimension.Xym)
                        {
                            geometryDimension = Dimension.Xym;
                            return new Point(double.Parse(coordinates[0], CultureInfo.InvariantCulture), double.Parse(coordinates[1], CultureInfo.InvariantCulture), null, double.Parse(coordinates[2], CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            geometryDimension = Dimension.Xyz;
                            return new Point(double.Parse(coordinates[0], CultureInfo.InvariantCulture), double.Parse(coordinates[1], CultureInfo.InvariantCulture), double.Parse(coordinates[2], CultureInfo.InvariantCulture));
                        }
                    }
                case 4: geometryDimension = Dimension.Xyzm; return new Point(double.Parse(coordinates[0], CultureInfo.InvariantCulture), double.Parse(coordinates[1], CultureInfo.InvariantCulture), double.Parse(coordinates[2], CultureInfo.InvariantCulture), double.Parse(coordinates[3], CultureInfo.InvariantCulture));
                default: throw new Exception("Expected coordinates");
            }
        }
    }
}
