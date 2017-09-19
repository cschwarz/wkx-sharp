using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Wkx
{
    internal class WktWriter
    {
        protected StringBuilder wktBuilder;

        internal WktWriter()
        {
            wktBuilder = new StringBuilder();
        }

        internal virtual string Write(Geometry geometry, bool skipType = false)
        {
            WriteWktType(geometry.GeometryType, geometry.Dimension, geometry.IsEmpty, skipType);

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
                case GeometryType.CircularString: WriteCircularString(geometry as CircularString); break;
                case GeometryType.CompoundCurve: WriteCompoundCurve(geometry as CompoundCurve); break;
                case GeometryType.CurvePolygon: WriteCurvePolygon(geometry as CurvePolygon); break;
                case GeometryType.MultiCurve: WriteMultiCurve(geometry as MultiCurve); break;
                case GeometryType.MultiSurface: WriteMultiSurface(geometry as MultiSurface); break;
                case GeometryType.PolyhedralSurface: WritePolyhedralSurface(geometry as PolyhedralSurface); break;
                case GeometryType.Tin: WriteTin(geometry as Tin); break;
                case GeometryType.Triangle: WriteTriangle(geometry as Triangle); break;
                default: throw new NotSupportedException(geometry.GeometryType.ToString());
            }

            return wktBuilder.ToString();
        }

        protected virtual void WriteWktType(GeometryType geometryType, Dimension dimension, bool isEmpty, bool skipType = false)
        {
            if (!skipType)
            {
                wktBuilder.Append(geometryType.ToString().ToUpperInvariant());

                switch (dimension)
                {
                    case Dimension.Xyz: wktBuilder.Append(" Z "); break;
                    case Dimension.Xym: wktBuilder.Append(" M "); break;
                    case Dimension.Xyzm: wktBuilder.Append(" ZM "); break;
                }
            }

            if (isEmpty && dimension == Dimension.Xy)
                wktBuilder.Append(" ");

            if (isEmpty)
                wktBuilder.Append("EMPTY");
        }

        private string GetWktCoordinate(Point coordinate)
        {
            if (coordinate.Z.HasValue && coordinate.M.HasValue)
                return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
            else if (coordinate.Z.HasValue)
                return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", coordinate.X, coordinate.Y, coordinate.Z);
            else if (coordinate.M.HasValue)
                return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", coordinate.X, coordinate.Y, coordinate.M);

            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", coordinate.X, coordinate.Y);
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

            WriteWktCoordinates(polygon.ExteriorRing.Points);
            wktBuilder.Append("),");

            foreach (LinearRing interiorRing in polygon.InteriorRings)
            {
                wktBuilder.Append("(");
                WriteWktCoordinates(interiorRing.Points);
                wktBuilder.Append("),");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteMultiPoint(MultiPoint multiPoint)
        {
            wktBuilder.Append("(");
            WriteWktCoordinates(multiPoint.Geometries);
            wktBuilder.Append(")");
        }

        private void WriteMultiLineString(MultiLineString multiLineString)
        {
            wktBuilder.Append("(");

            foreach (LineString lineString in multiLineString.Geometries)
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

            foreach (Polygon polygon in multiPolygon.Geometries)
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

        private void WriteCircularString(CircularString circularString)
        {
            wktBuilder.Append("(");
            WriteWktCoordinates(circularString.Points);
            wktBuilder.Append(")");
        }

        private void WriteCompoundCurve(CompoundCurve compoundCurve)
        {
            wktBuilder.Append("(");

            foreach (Geometry geometry in compoundCurve.Geometries)
            {
                Write(geometry, geometry.GeometryType == GeometryType.LineString);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteCurvePolygon(CurvePolygon curvePolygon)
        {
            wktBuilder.Append("(");
            Write(curvePolygon.ExteriorRing, curvePolygon.ExteriorRing.GeometryType == GeometryType.LineString);
            wktBuilder.Append(",");

            foreach (Curve interiorRing in curvePolygon.InteriorRings)
            {
                Write(interiorRing, interiorRing.GeometryType == GeometryType.LineString);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteMultiCurve(MultiCurve multiCurve)
        {
            wktBuilder.Append("(");

            foreach (Geometry geometry in multiCurve.Geometries)
            {
                Write(geometry, geometry.GeometryType == GeometryType.LineString);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteMultiSurface(MultiSurface multiSurface)
        {
            wktBuilder.Append("(");

            foreach (Geometry geometry in multiSurface.Geometries)
            {
                Write(geometry, geometry.GeometryType == GeometryType.Polygon);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WritePolyhedralSurface(PolyhedralSurface polyhedralSurface)
        {
            wktBuilder.Append("(");

            foreach (Geometry geometry in polyhedralSurface.Geometries)
            {
                Write(geometry, true);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteTin(Tin tin)
        {
            wktBuilder.Append("(");

            foreach (Geometry geometry in tin.Geometries)
            {
                Write(geometry, true);
                wktBuilder.Append(",");
            }

            wktBuilder.Remove(wktBuilder.Length - 1, 1);
            wktBuilder.Append(")");
        }

        private void WriteTriangle(Triangle triangle)
        {
            WritePolygon(triangle);
        }
    }
}
