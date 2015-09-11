using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wkx
{
    internal class WkbReader
    {
        private BinaryReader wkbReader;

        internal WkbReader(Stream stream)
        {
            wkbReader = new BinaryReader(stream);
        }

        internal Geometry Read()
        {
            bool isBigEndian = wkbReader.ReadBoolean();
            GeometryType geometryType = (GeometryType)wkbReader.ReadUInt32();

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

        private T Read<T>() where T : Geometry
        {
            return (T)Read();
        }

        private Point ReadPoint()
        {
            return new Point(wkbReader.ReadDouble(), wkbReader.ReadDouble());
        }

        private LineString ReadLineString()
        {
            LineString lineString = new LineString();

            uint pointCount = wkbReader.ReadUInt32();

            for (int i = 0; i < pointCount; i++)
                lineString.Points.Add(ReadPoint());

            return lineString;
        }

        private Polygon ReadPolygon()
        {
            Polygon polygon = new Polygon();

            uint ringCount = wkbReader.ReadUInt32();

            if (ringCount > 0)
            {
                uint exteriorRingCount = wkbReader.ReadUInt32();
                for (int i = 0; i < exteriorRingCount; i++)
                    polygon.ExteriorRing.Add(ReadPoint());

                for (int i = 1; i < ringCount; i++)
                {
                    polygon.InteriorRings.Add(new List<Point>());

                    uint interiorRingCount = wkbReader.ReadUInt32();
                    for (int j = 0; j < interiorRingCount; j++)
                        polygon.InteriorRings[i - 1].Add(ReadPoint());
                }
            }

            return polygon;
        }

        private MultiPoint ReadMultiPoint()
        {
            MultiPoint multiPoint = new MultiPoint();

            uint pointCount = wkbReader.ReadUInt32();

            for (int i = 0; i < pointCount; i++)
                multiPoint.Points.Add(Read<Point>());

            return multiPoint;
        }

        private MultiLineString ReadMultiLineString()
        {
            MultiLineString multiLineString = new MultiLineString();

            uint lineStringCount = wkbReader.ReadUInt32();

            for (int i = 0; i < lineStringCount; i++)
                multiLineString.LineStrings.Add(Read<LineString>());

            return multiLineString;
        }

        private MultiPolygon ReadMultiPolygon()
        {
            MultiPolygon multiPolygon = new MultiPolygon();

            uint polygonCount = wkbReader.ReadUInt32();

            for (int i = 0; i < polygonCount; i++)
                multiPolygon.Polygons.Add(Read<Polygon>());

            return multiPolygon;
        }

        private GeometryCollection ReadGeometryCollection()
        {
            GeometryCollection geometryCollection = new GeometryCollection();

            uint geometryCount = wkbReader.ReadUInt32();

            for (int i = 0; i < geometryCount; i++)
                geometryCollection.Geometries.Add(Read());

            return geometryCollection;
        }
    }
}
