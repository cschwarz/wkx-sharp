using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wkx
{
    internal class WktWriter
    {
        private StringBuilder wktBuilder;

        internal WktWriter()
        {
            wktBuilder = new StringBuilder();
        }

        internal virtual string Write(Geometry geometry)
        {
            WriteWktType(geometry.GeometryType, geometry.Dimensions, geometry.IsEmpty);

            if (geometry.IsEmpty)
                return wktBuilder.ToString();

            switch (geometry.GeometryType)
            {
                case GeometryType.Point: WritePoint(geometry as Point); break;
                case GeometryType.LineString: WriteLineString(geometry as LineString); break;
                case GeometryType.Polygon: WritePolygon(geometry as Polygon); break;
                case GeometryType.MultiPoint: WriteMultiPoint(geometry as MultiPoint); break;
                case GeometryType.MultiLineString: WriteMultiLineString(geometry as MultiLineString); break;
                case GeometryType.MultiPolygon: WriteMultiPolygon(geometry as MultiPolygon); break;
                case GeometryType.GeometryCollection: WriteGeometryCollection(geometry as GeometryCollection); break;
                default: throw new Exception();
            }

            return wktBuilder.ToString();
        }

        private void WriteWktType(GeometryType geometryType, Dimensions dimensions, bool isEmpty)
        {
            wktBuilder.Append(geometryType.ToString().ToUpperInvariant());

            switch (dimensions)
            {
                case Dimensions.XYZ: wktBuilder.Append(" Z "); break;
                case Dimensions.XYM: wktBuilder.Append(" M "); break;
                case Dimensions.XYZM: wktBuilder.Append(" ZM "); break;
            }

            if (isEmpty && dimensions == Dimensions.XY)
                wktBuilder.Append(" ");

            if (isEmpty)
                wktBuilder.Append("EMPTY");
        }

        private string GetWktCoordinate(Point coordinate)
        {
            if (coordinate.Z.HasValue && coordinate.M.HasValue)
                return string.Format("{0} {1} {2} {3}", coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
            else if (coordinate.Z.HasValue)
                return string.Format("{0} {1} {2}", coordinate.X, coordinate.Y, coordinate.Z);
            else if (coordinate.M.HasValue)
                return string.Format("{0} {1} {2}", coordinate.X, coordinate.Y, coordinate.M);

            return string.Format("{0} {1}", coordinate.X, coordinate.Y);
        }

        private void WriteWktCoordinates(IEnumerable<Point> coordinates)
        {
            wktBuilder.Append(string.Join(",", coordinates.Select(c => GetWktCoordinate(c))));
        }

        private void WritePoint(Point point)
        {
            wktBuilder.AppendFormat("({0})", GetWktCoordinate(point));
        }

        private void WriteLineString(LineString lineString)
        {
            wktBuilder.Append("(");
            WriteWktCoordinates(lineString.Points);
            wktBuilder.Append(")");
        }

        private void WritePolygon(Polygon polygon)
        {
            wktBuilder.Append("((");

            WriteWktCoordinates(polygon.ExteriorRing);
            wktBuilder.Append("),");

            foreach (List<Point> interiorRing in polygon.InteriorRings)
            {
                wktBuilder.Append("(");
                WriteWktCoordinates(interiorRing);
                wktBuilder.Append("),");
            }
            
            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteMultiPoint(MultiPoint multiPoint)
        {
            wktBuilder.Append("(");
            WriteWktCoordinates(multiPoint.Points);
            wktBuilder.Append(")");
        }

        private void WriteMultiLineString(MultiLineString multiLineString)
        {
            wktBuilder.Append("(");

            foreach (LineString lineString in multiLineString.LineStrings)
            {
                WriteLineString(lineString);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteMultiPolygon(MultiPolygon multiPolygon)
        {
            wktBuilder.Append("(");

            foreach (Polygon polygon in multiPolygon.Polygons)
            {
                WritePolygon(polygon);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteGeometryCollection(GeometryCollection geometryCollection)
        {
            wktBuilder.Append("(");

            foreach (Geometry geometry in geometryCollection.Geometries)
            {
                Write(geometry);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }
    }
}
