using System;
using System.IO;

namespace Wkx
{
    internal class WkbWriter
    {
        protected static readonly byte[] doubleNaN = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf8, 0x7f };

        protected BinaryWriter wkbWriter;

        internal byte[] Write(Geometry geometry)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wkbWriter = new BinaryWriter(memoryStream);
                WriteInternal(geometry);

                return memoryStream.ToArray();
            }
        }

        protected virtual void WriteWkbType(GeometryType geometryType, Dimension dimension, int? srid)
        {
            uint dimensionType = 0;

            switch (dimension)
            {
                case Dimension.Xyz: dimensionType = 1000; break;
                case Dimension.Xym: dimensionType = 2000; break;
                case Dimension.Xyzm: dimensionType = 3000; break;
            }

            wkbWriter.Write(dimensionType + (uint)geometryType);
        }

        private void WriteInternal(Geometry geometry, Geometry parentGeometry = null)
        {
            wkbWriter.Write(true);

            Dimension dimension = parentGeometry != null ? parentGeometry.Dimension : geometry.Dimension;

            WriteWkbType(geometry.GeometryType, dimension, geometry.Srid);

            switch (geometry.GeometryType)
            {
                case GeometryType.Point: WritePoint(geometry as Point, dimension); break;
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
        }

        private void WritePoint(Point point, Dimension dimension)
        {
            WriteDouble(point.X);
            WriteDouble(point.Y);

            if (dimension == Dimension.Xyz || dimension == Dimension.Xyzm)
                WriteDouble(point.Z);

            if (dimension == Dimension.Xym || dimension == Dimension.Xyzm)
                WriteDouble(point.M);
        }

        private void WriteDouble(double? value)
        {
            if (value.HasValue)
                wkbWriter.Write(value.Value);
            else
                wkbWriter.Write(doubleNaN);
        }

        private void WriteLineString(LineString lineString)
        {
            wkbWriter.Write(lineString.Points.Count);

            foreach (Point point in lineString.Points)
                WritePoint(point, lineString.Dimension);
        }

        private void WritePolygon(Polygon polygon)
        {
            if (polygon.IsEmpty)
            {
                wkbWriter.Write(0);
                return;
            }

            wkbWriter.Write(1 + polygon.InteriorRings.Count);

            wkbWriter.Write(polygon.ExteriorRing.Points.Count);
            foreach (Point point in polygon.ExteriorRing.Points)
                WritePoint(point, polygon.Dimension);

            foreach (LinearRing interiorRing in polygon.InteriorRings)
            {
                wkbWriter.Write(interiorRing.Points.Count);
                foreach (Point point in interiorRing.Points)
                    WritePoint(point, polygon.Dimension);
            }
        }

        private void WriteMultiPoint(MultiPoint multiPoint)
        {
            wkbWriter.Write(multiPoint.Geometries.Count);

            foreach (Point point in multiPoint.Geometries)
                WriteInternal(point, multiPoint);
        }

        private void WriteMultiLineString(MultiLineString multiLineString)
        {
            wkbWriter.Write(multiLineString.Geometries.Count);

            foreach (LineString lineString in multiLineString.Geometries)
                WriteInternal(lineString, multiLineString);
        }

        private void WriteMultiPolygon(MultiPolygon multiPolygon)
        {
            wkbWriter.Write(multiPolygon.Geometries.Count);

            foreach (Polygon polygon in multiPolygon.Geometries)
                WriteInternal(polygon, multiPolygon);
        }

        private void WriteGeometryCollection(GeometryCollection geometryCollection)
        {
            wkbWriter.Write(geometryCollection.Geometries.Count);

            foreach (Geometry geometry in geometryCollection.Geometries)
                WriteInternal(geometry);
        }

        private void WriteCircularString(CircularString circularString)
        {
            wkbWriter.Write(circularString.Points.Count);

            foreach (Point point in circularString.Points)
                WritePoint(point, circularString.Dimension);
        }

        private void WriteCompoundCurve(CompoundCurve compoundCurve)
        {
            wkbWriter.Write(compoundCurve.Geometries.Count);

            foreach (Geometry geometry in compoundCurve.Geometries)
                WriteInternal(geometry);
        }

        private void WriteCurvePolygon(CurvePolygon curvePolygon)
        {
            if (curvePolygon.IsEmpty)
            {
                wkbWriter.Write(0);
                return;
            }

            wkbWriter.Write(1 + curvePolygon.InteriorRings.Count);

            WriteInternal(curvePolygon.ExteriorRing);
            
            foreach (Curve interiorRing in curvePolygon.InteriorRings)
                WriteInternal(interiorRing);
        }

        private void WriteMultiCurve(MultiCurve multiCurve)
        {
            wkbWriter.Write(multiCurve.Geometries.Count);

            foreach (Geometry geometry in multiCurve.Geometries)
                WriteInternal(geometry);
        }

        private void WriteMultiSurface(MultiSurface multiSurface)
        {
            wkbWriter.Write(multiSurface.Geometries.Count);

            foreach (Geometry geometry in multiSurface.Geometries)
                WriteInternal(geometry);
        }

        private void WritePolyhedralSurface(PolyhedralSurface polyhedralSurface)
        {
            wkbWriter.Write(polyhedralSurface.Geometries.Count);

            foreach (Geometry geometry in polyhedralSurface.Geometries)
                WriteInternal(geometry);
        }

        private void WriteTin(Tin tin)
        {
            wkbWriter.Write(tin.Geometries.Count);

            foreach (Geometry geometry in tin.Geometries)
                WriteInternal(geometry);
        }
        
        private void WriteTriangle(Triangle triangle)
        {
            if (triangle.IsEmpty)
            {
                wkbWriter.Write(0);
                return;
            }

            wkbWriter.Write(1 + triangle.InteriorRings.Count);

            wkbWriter.Write(triangle.ExteriorRing.Points.Count);
            foreach (Point point in triangle.ExteriorRing.Points)
                WritePoint(point, triangle.Dimension);

            foreach (LinearRing interiorRing in triangle.InteriorRings)
            {
                wkbWriter.Write(interiorRing.Points.Count);
                foreach (Point point in interiorRing.Points)
                    WritePoint(point, triangle.Dimension);
            }
        }
    }
}
