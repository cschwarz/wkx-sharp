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

            return Read(geometryType, dimension);
        }

        protected virtual Geometry ReadType(GeometryType defaultType, Dimension defaultDimension, params GeometryType[] types)
        {
            GeometryType geometryType = MatchTypeOrDefault(defaultType, types);
            Dimension dimension = MatchDimension(defaultDimension);

            Geometry geometry = CreateGeometry(geometryType, dimension);

            if (IsEmpty())
                return geometry;

            return Read(geometryType, dimension);
        }

        protected Geometry Read(GeometryType geometryType, Dimension dimension)
        {
            switch (geometryType)
            {
                case GeometryType.Point: return ReadPoint(dimension);
                case GeometryType.LineString: return ReadLineString(dimension);
                case GeometryType.Polygon: return ReadPolygon(dimension);
                case GeometryType.MultiPoint: return ReadMultiPoint(dimension);
                case GeometryType.MultiLineString: return ReadMultiLineString(dimension);
                case GeometryType.MultiPolygon: return ReadMultiPolygon(dimension);
                case GeometryType.GeometryCollection: return ReadGeometryCollection(dimension);
                case GeometryType.CircularString: return ReadCircularString(dimension);
                case GeometryType.CompoundCurve: return ReadCompoundCurve(dimension);
                case GeometryType.CurvePolygon: return ReadCurvePolygon(dimension);
                case GeometryType.MultiCurve: return ReadMultiCurve(dimension);
                case GeometryType.MultiSurface: return ReadMultiSurface(dimension);
                case GeometryType.PolyhedralSurface: return ReadPolyhedralSurface(dimension);
                case GeometryType.Tin: return ReadTin(dimension);
                case GeometryType.Triangle: return ReadTriangle(dimension);
                default: throw new NotSupportedException(geometryType.ToString());
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
                case GeometryType.CircularString: geometry = new CircularString(); break;
                case GeometryType.CompoundCurve: geometry = new CompoundCurve(); break;
                case GeometryType.CurvePolygon: geometry = new CurvePolygon(); break;
                case GeometryType.MultiCurve: geometry = new MultiCurve(); break;
                case GeometryType.MultiSurface: geometry = new MultiSurface(); break;
                case GeometryType.PolyhedralSurface: geometry = new PolyhedralSurface(); break;
                case GeometryType.Tin: geometry = new Tin(); break;
                case GeometryType.Triangle: geometry = new Triangle(); break;
                default: throw new NotSupportedException(geometryType.ToString());
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
                polygon.InteriorRings.Add(new LinearRing(new List<Point>(MatchCoordinates(dimension))));
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
                multiLineString.Geometries.Add(ReadLineString(dimension));
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
                multiPolygon.Geometries.Add(ReadPolygon(dimension));
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

        protected CircularString ReadCircularString(Dimension dimension)
        {
            ExpectGroupStart();
            CircularString circularString = new CircularString(MatchCoordinates(dimension));
            circularString.Dimension = dimension;
            ExpectGroupEnd();

            return circularString;
        }

        protected CompoundCurve ReadCompoundCurve(Dimension dimension)
        {
            ExpectGroupStart();
            CompoundCurve compoundCurve = new CompoundCurve();
            compoundCurve.Dimension = dimension;

            do
            {
                compoundCurve.Geometries.Add((Curve)ReadType(GeometryType.LineString, dimension, GeometryType.LineString, GeometryType.CircularString));
            } while (IsMatch(","));


            ExpectGroupEnd();

            return compoundCurve;
        }

        protected CurvePolygon ReadCurvePolygon(Dimension dimension)
        {
            ExpectGroupStart();
            CurvePolygon curvePolygon = new CurvePolygon((Curve)ReadType(GeometryType.LineString, dimension, GeometryType.LineString, GeometryType.CircularString, GeometryType.CompoundCurve));
            curvePolygon.Dimension = dimension;

            while (IsMatch(","))
            {
                curvePolygon.InteriorRings.Add((Curve)ReadType(GeometryType.LineString, dimension, GeometryType.LineString, GeometryType.CircularString, GeometryType.CompoundCurve));
            }

            ExpectGroupEnd();

            return curvePolygon;
        }

        protected MultiCurve ReadMultiCurve(Dimension dimension)
        {
            ExpectGroupStart();
            MultiCurve multiCurve = new MultiCurve();
            multiCurve.Dimension = dimension;

            do
            {
                multiCurve.Geometries.Add((Curve)ReadType(GeometryType.LineString, dimension, GeometryType.LineString, GeometryType.CircularString, GeometryType.CompoundCurve));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiCurve;
        }

        protected MultiSurface ReadMultiSurface(Dimension dimension)
        {
            ExpectGroupStart();
            MultiSurface multiSurface = new MultiSurface();
            multiSurface.Dimension = dimension;

            do
            {
                multiSurface.Geometries.Add((Surface)ReadType(GeometryType.Polygon, dimension, GeometryType.Polygon, GeometryType.CurvePolygon));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return multiSurface;
        }

        protected PolyhedralSurface ReadPolyhedralSurface(Dimension dimension)
        {
            PolyhedralSurface polyhedralSurface = new PolyhedralSurface();
            polyhedralSurface.Dimension = dimension;

            ExpectGroupStart();

            do
            {
                polyhedralSurface.Geometries.Add(ReadPolygon(dimension));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return polyhedralSurface;
        }

        protected Tin ReadTin(Dimension dimension)
        {
            Tin tin = new Tin();
            tin.Dimension = dimension;

            ExpectGroupStart();

            do
            {
                tin.Geometries.Add(ReadTriangle(dimension));
            } while (IsMatch(","));

            ExpectGroupEnd();

            return tin;
        }

        protected Triangle ReadTriangle(Dimension dimension)
        {
            ExpectGroupStart();

            ExpectGroupStart();
            Triangle triangle = new Triangle(MatchCoordinates(dimension));
            triangle.Dimension = dimension;
            ExpectGroupEnd();

            while (IsMatch(","))
            {
                ExpectGroupStart();
                triangle.InteriorRings.Add(new LinearRing(new List<Point>(MatchCoordinates(dimension))));
                ExpectGroupEnd();
            }

            ExpectGroupEnd();

            return triangle;
        }

        protected GeometryType MatchType()
        {
            GeometryType geometryType;

            if (!Enum.TryParse<GeometryType>(Match(wktTypes), true, out geometryType))
                throw new Exception("Expected geometry type");

            return geometryType;
        }

        protected GeometryType MatchTypeOrDefault(GeometryType defaultType, params GeometryType[] types)
        {
            GeometryType geometryType;

            if (!Enum.TryParse<GeometryType>(Match(types.Select(t => t.ToString().ToUpperInvariant()).ToArray()), true, out geometryType))
                return defaultType;

            return geometryType;
        }

        protected Dimension MatchDimension(Dimension? defaultDimension = null)
        {
            string dimensionMatch = Match("ZM", "Z", "M");

            if (dimensionMatch == null)
                return defaultDimension.HasValue ? defaultDimension.Value : Dimension.Xy;

            switch (dimensionMatch)
            {
                case "Z": return Dimension.Xyz;
                case "M": return Dimension.Xym;
                case "ZM": return Dimension.Xyzm;
                default: throw new NotSupportedException(dimensionMatch.ToString());
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
                            dimension = Dimension.Xym;
                            return new Point(double.Parse(coordinates[0], CultureInfo.InvariantCulture), double.Parse(coordinates[1], CultureInfo.InvariantCulture), null, double.Parse(coordinates[2], CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            dimension = Dimension.Xyz;
                            return new Point(double.Parse(coordinates[0], CultureInfo.InvariantCulture), double.Parse(coordinates[1], CultureInfo.InvariantCulture), double.Parse(coordinates[2], CultureInfo.InvariantCulture));
                        }
                    }
                case 4: dimension = Dimension.Xyzm; return new Point(double.Parse(coordinates[0], CultureInfo.InvariantCulture), double.Parse(coordinates[1], CultureInfo.InvariantCulture), double.Parse(coordinates[2], CultureInfo.InvariantCulture), double.Parse(coordinates[3], CultureInfo.InvariantCulture));
                default: throw new Exception("Expected coordinates");
            }
        }

        protected IEnumerable<Point> MatchCoordinates(Dimension dimension)
        {
            List<Point> coordinates = new List<Point>();

            do
            {
                bool startsWithBracket = IsMatch("(");

                coordinates.Add(MatchCoordinate(dimension));

                if (startsWithBracket)
                    ExpectGroupEnd();
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

        protected string Peek(params char[] tokens)
        {
            SkipWhitespaces();

            int tokenPosition = position;
            bool foundToken = false;

            do
            {
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (tokenPosition + 1 < value.Length && value[tokenPosition + 1] == tokens[i])
                    {
                        foundToken = true;
                        break;
                    }
                }

                tokenPosition++;
            } while (!foundToken && tokenPosition < value.Length);

            string peekValue = value.Substring(position, tokenPosition - position);
            position = tokenPosition;
            return peekValue;
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
                if (position + tokens[i].Length <= value.Length && value.Substring(position, tokens[i].Length) == tokens[i])
                {
                    position += tokens[i].Length;
                    return true;
                }
            }

            return false;
        }
    }
}
