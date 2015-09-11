using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wkx
{
    internal class WkbWriter
    {
        private BinaryWriter wkbWriter;
        
        internal byte[] Write(Geometry geometry)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wkbWriter = new BinaryWriter(memoryStream);
                WriteInternal(geometry);

                return memoryStream.ToArray();
            }               
        }

        private void WriteInternal(Geometry geometry)
        {
            wkbWriter.Write(true);

            if (geometry.GeometryType == GeometryType.Point && geometry.IsEmpty)
            {
                wkbWriter.Write((uint)GeometryType.MultiPoint);
                wkbWriter.Write(0);
            }
            else
            {
                wkbWriter.Write((uint)geometry.GeometryType);

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
            }
        }

        private void WritePoint(Point point)
        {
            wkbWriter.Write(point.X.Value);
            wkbWriter.Write(point.Y.Value);
        }

        private void WriteLineString(LineString lineString)
        {
            wkbWriter.Write(lineString.Points.Count);

            foreach (Point point in lineString.Points)
                WritePoint(point);
        }

        private void WritePolygon(Polygon polygon)
        {
            if (polygon.IsEmpty)
            {
                wkbWriter.Write(0);
                return;
            }

            wkbWriter.Write(1 + polygon.InteriorRings.Count);

            wkbWriter.Write(polygon.ExteriorRing.Count);
            foreach (Point point in polygon.ExteriorRing)
                WritePoint(point);

            foreach (List<Point> interiorRing in polygon.InteriorRings)
            {
                wkbWriter.Write(interiorRing.Count);
                foreach (Point point in interiorRing)
                    WritePoint(point);
            }
        }

        private void WriteMultiPoint(MultiPoint multiPoint)
        {
            wkbWriter.Write(multiPoint.Points.Count);

            foreach (Point point in multiPoint.Points)
                WriteInternal(point);
        }

        private void WriteMultiLineString(MultiLineString multiLineString)
        {
            wkbWriter.Write(multiLineString.LineStrings.Count);

            foreach (LineString lineString in multiLineString.LineStrings)
                WriteInternal(lineString);
        }

        private void WriteMultiPolygon(MultiPolygon multiPolygon)
        {
            wkbWriter.Write(multiPolygon.Polygons.Count);

            foreach (Polygon polygon in multiPolygon.Polygons)
                WriteInternal(polygon);
        }

        private void WriteGeometryCollection(GeometryCollection geometryCollection)
        {
            wkbWriter.Write(geometryCollection.Geometries.Count);

            foreach (Geometry geometry in geometryCollection.Geometries)
                WriteInternal(geometry);
        }
    }
}
