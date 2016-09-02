using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Wkx
{
    internal class WktReader
    {
        private static readonly string[] wktTypes;

        private string value;
        private int position;

        static WktReader()
        {
            wktTypes = Enum.GetValues(typeof(GeometryType)).Cast<GeometryType>().Select(g => g.ToString().ToUpperInvariant()).ToArray();
        }

        internal WktReader(string value)
        {
            this.value = value;
        }

        internal virtual Geometry Read()
        {
            GeometryType geometryType = MatchType();
            Dimension dimension = MatchDimension();

            Geometry geometry = CreateGeometry(geometryType, dimension);

            if (IsEmpty())
                return geometry;

            switch (geometryType)
            {
                case GeometryType.Point: return ReadPoint(dimension);
                case GeometryType.LineString: return ReadLineString(dimension);
                case GeometryType.Polygon: return ReadPolygon(dimension);
                case GeometryType.MultiPoint: return ReadMultiPoint(dimension);
                case GeometryType.MultiLineString: return ReadMultiLineString(dimension);
                case GeometryType.MultiPolygon: return ReadMultiPolygon(dimension);
                case GeometryType.GeometryCollection: return ReadGeometryCollection(dimension);
                default: throw new Exception();
            }
        }

        protected Geometry CreateGeometry(GeometryType geometryType, Dimension dimension)
        {
            Geometry geometry = null;

            switch (geometryType)
            {
                case GeometryType.Point: geometry = new Point(); break;
                case GeometryType.LineString: geometry = new LineString(); break;
                case GeometryType.Polygon: geometry = new Polygon(); break;
                case GeometryType.MultiPoint: geometry = new MultiPoint(); break;
                case GeometryType.MultiLineString: geometry = new MultiLineString(); break;
                case GeometryType.MultiPolygon: geometry = new MultiPolygon(); break;
                case GeometryType.GeometryCollection: geometry = new GeometryCollection(); break;
            }

            geometry.Dimension = dimension;

            return geometry;
        }

        protected Point ReadPoint(Dimension dimension)
        {
            ExpectGroupStart();
            Point point = MatchCoordinate(dimension);
            point.Dimension = dimension;
            ExpectGroupEnd();

            return point;
        }

        protected LineString ReadLineString(Dimension dimension)
        {
            ExpectGroupStart();
            LineString lineString = new LineString(MatchCoordinates(dimension));
            lineString.Dimension = dimension;
            ExpectGroupEnd();

            return lineString;
        }

        protected Polygon ReadPolygon(Dimension dimension)
        {
            ExpectGroupStart();

            ExpectGroupStart();
            Polygon polygon = new Polygon(MatchCoordinates(dimension));
            polygon.Dimension = dimension;
            ExpectGroupEnd();

            while (IsMatch(","))
            {
                ExpectGroupStart();
                polygon.InteriorRings.Add(new List<Point>(MatchCoordinates(dimension)));
                ExpectGroupEnd();
            }

            ExpectGroupEnd();

            return polygon;
        }

        protected MultiPoint ReadMultiPoint(Dimension dimension)
        {
            ExpectGroupStart();
            MultiPoint multiPoint = new MultiPoint(MatchCoordinates(dimension));
            multiPoint.Dimension = dimension;
            ExpectGroupEnd();

            return multiPoint;
        }

        protected MultiLineString ReadMultiLineString(Dimension dimension)
        {
            MultiLineString multiLineString = new MultiLineString();
            multiLineString.Dimension = dimension;

            ExpectGroupStart();

            do
            {
                multiLineString.LineStrings.Add(ReadLineString(dimension));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiLineString;
        }

        protected MultiPolygon ReadMultiPolygon(Dimension dimension)
        {
            MultiPolygon multiPolygon = new MultiPolygon();
            multiPolygon.Dimension = dimension;

            ExpectGroupStart();

            do
            {
                multiPolygon.Polygons.Add(ReadPolygon(dimension));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiPolygon;
        }

        protected GeometryCollection ReadGeometryCollection(Dimension dimension)
        {
            GeometryCollection geometryCollection = new GeometryCollection();
            geometryCollection.Dimension = dimension;

            ExpectGroupStart();

            do
            {
                geometryCollection.Geometries.Add(Read());
            } while (IsMatch(","));

            ExpectGroupEnd();

            return geometryCollection;
        }

        protected GeometryType MatchType()
        {
            GeometryType geometryType;

            if (!Enum.TryParse<GeometryType>(Match(wktTypes), true, out geometryType))
                throw new Exception("Expected geometry type");

            return geometryType;
        }

        protected Dimension MatchDimension()
        {
            string dimensionMatch = Match("ZM", "Z", "M");

            if (dimensionMatch == null)
                return Dimension.Xy;

            switch (dimensionMatch)
            {
                case "Z": return Dimension.Xyz;
                case "M": return Dimension.Xym;
                case "ZM": return Dimension.Xyzm;
                default: throw new Exception();
            }
        }

        protected void ExpectGroupStart()
        {
            if (!IsMatch("("))
                throw new Exception("Expected group start");
        }

        protected void ExpectGroupEnd()
        {
            if (!IsMatch(")"))
                throw new Exception("Expected group end");
        }

        protected virtual Point MatchCoordinate(Dimension dimension)
        {
            Match match = null;

            switch (dimension)
            {
                case Dimension.Xy: match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)"); break;
                case Dimension.Xyz:
                case Dimension.Xym: match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)"); break;
                case Dimension.Xyzm: match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)"); break;
                default: throw new Exception();
            }

            if (!match.Success)
                throw new Exception("Expected coordinates");

            switch (dimension)
            {
                case Dimension.Xy: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
                case Dimension.Xyz: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                case Dimension.Xym: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), null, double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                case Dimension.Xyzm: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
                default: throw new Exception();
            }
        }

        protected IEnumerable<Point> MatchCoordinates(Dimension dimension)
        {
            List<Point> coordinates = new List<Point>();

            do
            {
                coordinates.Add(MatchCoordinate(dimension));
            } while (IsMatch(","));

            return coordinates;
        }

        protected bool IsEmpty()
        {
            return IsMatch("EMPTY");
        }

        protected void SkipWhitespaces()
        {
            while (position < value.Length && char.IsWhiteSpace(value[position]))
                position++;
        }

        protected string Match(params string[] tokens)
        {
            SkipWhitespaces();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (value.IndexOf(tokens[i], position) == position)
                {
                    position += tokens[i].Length;
                    return tokens[i];
                }
            }

            return null;
        }

        protected Match MatchRegex(params string[] tokens)
        {
            SkipWhitespaces();

            string subValue = value.Substring(position);

            for (int i = 0; i < tokens.Length; i++)
            {
                Match match = Regex.Match(subValue, tokens[i]);

                if (match != null)
                {
                    position += match.Length;
                    return match;
                }
            }

            return null;
        }

        protected bool IsMatch(params string[] tokens)
        {
            SkipWhitespaces();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (value.IndexOf(tokens[i], position) == position)
                {
                    position += tokens[i].Length;
                    return true;
                }
            }

            return false;
        }
    }
}
