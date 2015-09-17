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
            Dimensions dimensions = MatchDimensions();

            Geometry geometry = CreateGeometry(geometryType, dimensions);

            if (IsEmpty())
                return geometry;

            switch (geometryType)
            {
                case GeometryType.Point: return ReadPoint(dimensions);
                case GeometryType.LineString: return ReadLineString(dimensions);
                case GeometryType.Polygon: return ReadPolygon(dimensions);
                case GeometryType.MultiPoint: return ReadMultiPoint(dimensions);
                case GeometryType.MultiLineString: return ReadMultiLineString(dimensions);
                case GeometryType.MultiPolygon: return ReadMultiPolygon(dimensions);
                case GeometryType.GeometryCollection: return ReadGeometryCollection(dimensions);
                default: throw new Exception();
            }
        }

        protected Geometry CreateGeometry(GeometryType geometryType, Dimensions dimensions)
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

            geometry.Dimensions = dimensions;

            return geometry;
        }
        
        protected Point ReadPoint(Dimensions dimensions)
        {
            ExpectGroupStart();
            Point point = MatchCoordinate(dimensions);
            point.Dimensions = dimensions;
            ExpectGroupEnd();

            return point;
        }

        protected LineString ReadLineString(Dimensions dimensions)
        {
            ExpectGroupStart();
            LineString lineString = new LineString(MatchCoordinates(dimensions));
            lineString.Dimensions = dimensions;
            ExpectGroupEnd();

            return lineString;
        }

        protected Polygon ReadPolygon(Dimensions dimensions)
        {
            ExpectGroupStart();

            ExpectGroupStart();
            Polygon polygon = new Polygon(MatchCoordinates(dimensions));
            polygon.Dimensions = dimensions;
            ExpectGroupEnd();

            while (IsMatch(","))
            {
                ExpectGroupStart();
                polygon.InteriorRings.Add(new List<Point>(MatchCoordinates(dimensions)));
                ExpectGroupEnd();
            }

            ExpectGroupEnd();

            return polygon;
        }

        protected MultiPoint ReadMultiPoint(Dimensions dimensions)
        {
            ExpectGroupStart();
            MultiPoint multiPoint = new MultiPoint(MatchCoordinates(dimensions));
            multiPoint.Dimensions = dimensions;
            ExpectGroupEnd();

            return multiPoint;
        }

        protected MultiLineString ReadMultiLineString(Dimensions dimensions)
        {
            MultiLineString multiLineString = new MultiLineString();
            multiLineString.Dimensions = dimensions;

            ExpectGroupStart();

            do
            {
                multiLineString.LineStrings.Add(ReadLineString(dimensions));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiLineString;
        }

        protected MultiPolygon ReadMultiPolygon(Dimensions dimensions)
        {
            MultiPolygon multiPolygon = new MultiPolygon();
            multiPolygon.Dimensions = dimensions;

            ExpectGroupStart();

            do
            {
                multiPolygon.Polygons.Add(ReadPolygon(dimensions));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiPolygon;
        }

        protected GeometryCollection ReadGeometryCollection(Dimensions dimensions)
        {
            GeometryCollection geometryCollection = new GeometryCollection();
            geometryCollection.Dimensions = dimensions;

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

        protected Dimensions MatchDimensions()
        {
            string dimensionsMatch = Match("ZM", "Z", "M");

            if (dimensionsMatch == null)
                return Dimensions.XY;

            switch(dimensionsMatch)
            {
                case "Z": return Dimensions.XY | Dimensions.Z;
                case "M": return Dimensions.XY | Dimensions.M;
                case "ZM": return Dimensions.XY | Dimensions.Z | Dimensions.M;
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

        protected virtual Point MatchCoordinate(Dimensions dimensions)
        {
            Match match = null;

            switch (dimensions)
            {
                case Dimensions.XY: match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)"); break;
                case Dimensions.XYZ:
                case Dimensions.XYM: match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)"); break;
                case Dimensions.XYZM: match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)\s+(-?\d+\.?\d*)"); break;
                default: throw new Exception();
            }

            if (!match.Success)
                throw new Exception("Expected coordinates");

            switch (dimensions)
            {
                case Dimensions.XY: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
                case Dimensions.XYZ: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                case Dimensions.XYM: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), null, double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                case Dimensions.XYZM: return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture), double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
                default: throw new Exception();
            }
        }

        protected IEnumerable<Point> MatchCoordinates(Dimensions dimensions)
        {
            List<Point> coordinates = new List<Point>();

            do
            {
                coordinates.Add(MatchCoordinate(dimensions));
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
