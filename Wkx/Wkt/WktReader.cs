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

            switch (geometryType)
            {
                case GeometryType.Point: return ReadPoint();
                case GeometryType.LineString: return ReadLineString();
                case GeometryType.Polygon: return ReadPolygon();
                case GeometryType.MultiPoint: return ReadMultiPoint();
                case GeometryType.MultiLineString: return ReadMultiLineString();
                case GeometryType.MultiPolygon: return ReadMultiPolygon();
                case GeometryType.GeometryCollection: return ReadGeometryCollection();
                default: throw new Exception();
            }
        }

        protected Point ReadPoint()
        {
            if (IsEmpty())
                return new Point();

            ExpectGroupStart();
            Point point = MatchCoordinate();
            ExpectGroupEnd();

            return point;
        }

        protected LineString ReadLineString()
        {
            if (IsEmpty())
                return new LineString();

            ExpectGroupStart();
            LineString lineString = new LineString(MatchCoordinates());
            ExpectGroupEnd();

            return lineString;
        }

        protected Polygon ReadPolygon()
        {
            if (IsEmpty())
                return new Polygon();

            ExpectGroupStart();

            ExpectGroupStart();
            Polygon polygon = new Polygon(MatchCoordinates());
            ExpectGroupEnd();

            while (IsMatch(","))
            {
                ExpectGroupStart();
                polygon.InteriorRings.Add(new List<Point>(MatchCoordinates()));
                ExpectGroupEnd();
            }

            ExpectGroupEnd();

            return polygon;
        }

        protected MultiPoint ReadMultiPoint()
        {
            if (IsEmpty())
                return new MultiPoint();

            ExpectGroupStart();
            MultiPoint multiPoint = new MultiPoint(MatchCoordinates());
            ExpectGroupEnd();

            return multiPoint;
        }

        protected MultiLineString ReadMultiLineString()
        {
            if (IsEmpty())
                return new MultiLineString();

            MultiLineString multiLineString = new MultiLineString();

            ExpectGroupStart();

            do
            {
                multiLineString.LineStrings.Add(ReadLineString());
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiLineString;
        }

        protected MultiPolygon ReadMultiPolygon()
        {
            if (IsEmpty())
                return new MultiPolygon();

            MultiPolygon multiPolygon = new MultiPolygon();

            ExpectGroupStart();

            do
            {
                multiPolygon.Polygons.Add(ReadPolygon());
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiPolygon;
        }

        protected GeometryCollection ReadGeometryCollection()
        {
            if (IsEmpty())
                return new GeometryCollection();

            GeometryCollection geometryCollection = new GeometryCollection();

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

        protected Point MatchCoordinate()
        {
            Match match = MatchRegex(@"^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)");

            if (!match.Success)
                throw new Exception("Expected coordinates");

            return new Point(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
        }

        protected IEnumerable<Point> MatchCoordinates()
        {
            List<Point> coordinates = new List<Point>();

            do
            {
                coordinates.Add(MatchCoordinate());
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
