using System;
using System.IO;

namespace Wkx
{
    internal class WkbReader
    {
        protected EndianBinaryReader wkbReader;

        internal WkbReader(Stream stream)
        {
            wkbReader = new EndianBinaryReader(stream);
        }

        internal Geometry Read()
        {
            wkbReader.IsBigEndian = !wkbReader.ReadBoolean();
            uint type = wkbReader.ReadUInt32();
            GeometryType geometryType = ReadGeometryType(type);
            Dimension dimension = ReadDimension(type);
            int? srid = ReadSrid(type);

            Geometry geometry = null;

            switch (geometryType)
            {
                case GeometryType.Point: geometry = ReadPoint(dimension); break;
                case GeometryType.LineString: geometry = ReadLineString(dimension); break;
                case GeometryType.Polygon: geometry = ReadPolygon(dimension); break;
                case GeometryType.MultiPoint: geometry = ReadMultiPoint(dimension); break;
                case GeometryType.MultiLineString: geometry = ReadMultiLineString(dimension); break;
                case GeometryType.MultiPolygon: geometry = ReadMultiPolygon(dimension); break;
                case GeometryType.GeometryCollection: geometry = ReadGeometryCollection(dimension); break;
                case GeometryType.CircularString: geometry = ReadCircularString(dimension); break;
                case GeometryType.CompoundCurve: geometry = ReadCompoundCurve(dimension); break;
                case GeometryType.CurvePolygon: geometry = ReadCurvePolygon(dimension); break;
                case GeometryType.MultiCurve: geometry = ReadMultiCurve(dimension); break;
                case GeometryType.MultiSurface: geometry = ReadMultiSurface(dimension); break;
                case GeometryType.PolyhedralSurface: geometry = ReadPolyhedralSurface(dimension); break;
                case GeometryType.Tin: geometry = ReadTin(dimension); break;
                case GeometryType.Triangle: geometry = ReadTriangle(dimension); break;
                default: throw new NotSupportedException(geometryType.ToString());
            }

            geometry.Dimension = dimension;
            geometry.Srid = srid;

            return geometry;
        }

        protected virtual GeometryType ReadGeometryType(uint type)
        {
            return (GeometryType)(type % 1000);
        }

        protected virtual Dimension ReadDimension(uint type)
        {
            if (type >= 1000 && type < 2000)
                return Dimension.Xyz;
            else if (type >= 2000 && type < 3000)
                return Dimension.Xym;
            else if (type >= 3000 && type < 4000)
                return Dimension.Xyzm;

            return Dimension.Xy;
        }

        protected virtual int? ReadSrid(uint type)
        {
            return null;
        }

        private T Read<T>() where T : Geometry
        {
            return (T)Read();
        }

        private Point ReadPoint(Dimension dimension)
        {
            switch (dimension)
            {
                case Dimension.Xy: return new Point(wkbReader.ReadDouble(), wkbReader.ReadDouble());
                case Dimension.Xyz: return new Point(wkbReader.ReadDouble(), wkbReader.ReadDouble(), wkbReader.ReadDouble());
                case Dimension.Xym: return new Point(wkbReader.ReadDouble(), wkbReader.ReadDouble(), null, wkbReader.ReadDouble());
                case Dimension.Xyzm: return new Point(wkbReader.ReadDouble(), wkbReader.ReadDouble(), wkbReader.ReadDouble(), wkbReader.ReadDouble());
                default: throw new NotSupportedException(dimension.ToString());
            }
        }

        private LineString ReadLineString(Dimension dimension)
        {
            LineString lineString = new LineString();

            uint pointCount = wkbReader.ReadUInt32();

            for (int i = 0; i < pointCount; i++)
                lineString.Points.Add(ReadPoint(dimension));

            return lineString;
        }

        private Polygon ReadPolygon(Dimension dimension)
        {
            Polygon polygon = new Polygon();

            uint ringCount = wkbReader.ReadUInt32();

            if (ringCount > 0)
            {
                uint exteriorRingCount = wkbReader.ReadUInt32();
                for (int i = 0; i < exteriorRingCount; i++)
                    polygon.ExteriorRing.Points.Add(ReadPoint(dimension));

                for (int i = 1; i < ringCount; i++)
                {
                    polygon.InteriorRings.Add(new LinearRing());

                    uint interiorRingCount = wkbReader.ReadUInt32();
                    for (int j = 0; j < interiorRingCount; j++)
                        polygon.InteriorRings[i - 1].Points.Add(ReadPoint(dimension));
                }
            }

            return polygon;
        }

        private MultiPoint ReadMultiPoint(Dimension dimension)
        {
            MultiPoint multiPoint = new MultiPoint();

            uint pointCount = wkbReader.ReadUInt32();

            for (int i = 0; i < pointCount; i++)
                multiPoint.Geometries.Add(Read<Point>());

            return multiPoint;
        }

        private MultiLineString ReadMultiLineString(Dimension dimension)
        {
            MultiLineString multiLineString = new MultiLineString();

            uint lineStringCount = wkbReader.ReadUInt32();

            for (int i = 0; i < lineStringCount; i++)
                multiLineString.Geometries.Add(Read<LineString>());

            return multiLineString;
        }

        private MultiPolygon ReadMultiPolygon(Dimension dimension)
        {
            MultiPolygon multiPolygon = new MultiPolygon();

            uint polygonCount = wkbReader.ReadUInt32();

            for (int i = 0; i < polygonCount; i++)
                multiPolygon.Geometries.Add(Read<Polygon>());

            return multiPolygon;
        }

        private GeometryCollection ReadGeometryCollection(Dimension dimension)
        {
            GeometryCollection geometryCollection = new GeometryCollection();

            uint geometryCount = wkbReader.ReadUInt32();

            for (int i = 0; i < geometryCount; i++)
                geometryCollection.Geometries.Add(Read());

            return geometryCollection;
        }

        private CircularString ReadCircularString(Dimension dimension)
        {
            CircularString circularString = new CircularString();

            uint pointCount = wkbReader.ReadUInt32();

            for (int i = 0; i < pointCount; i++)
                circularString.Points.Add(ReadPoint(dimension));

            return circularString;
        }

        private CompoundCurve ReadCompoundCurve(Dimension dimension)
        {
            CompoundCurve compoundCurve = new CompoundCurve();

            uint geometryCount = wkbReader.ReadUInt32();

            for (int i = 0; i < geometryCount; i++)
                compoundCurve.Geometries.Add(Read<Curve>());

            return compoundCurve;
        }

        private CurvePolygon ReadCurvePolygon(Dimension dimension)
        {
            CurvePolygon curvePolygon = new CurvePolygon();

            uint ringCount = wkbReader.ReadUInt32();

            if (ringCount > 0)
            {
                curvePolygon = new CurvePolygon(Read<Curve>());

                for (int i = 1; i < ringCount; i++)
                    curvePolygon.InteriorRings.Add(Read<Curve>());
            }

            return curvePolygon;
        }

        private MultiCurve ReadMultiCurve(Dimension dimension)
        {
            MultiCurve multiCurve = new MultiCurve();

            uint geometryCount = wkbReader.ReadUInt32();

            for (int i = 0; i < geometryCount; i++)
                multiCurve.Geometries.Add(Read<Curve>());

            return multiCurve;
        }

        private MultiSurface ReadMultiSurface(Dimension dimension)
        {
            MultiSurface multiSurface = new MultiSurface();

            uint geometryCount = wkbReader.ReadUInt32();

            for (int i = 0; i < geometryCount; i++)
                multiSurface.Geometries.Add(Read<Surface>());

            return multiSurface;
        }

        private PolyhedralSurface ReadPolyhedralSurface(Dimension dimension)
        {
            PolyhedralSurface polyhedralSurface = new PolyhedralSurface();

            uint geometryCount = wkbReader.ReadUInt32();

            for (int i = 0; i < geometryCount; i++)
                polyhedralSurface.Geometries.Add(Read<Polygon>());

            return polyhedralSurface;
        }

        private Tin ReadTin(Dimension dimension)
        {
            Tin tin = new Tin();

            uint geometryCount = wkbReader.ReadUInt32();

            for (int i = 0; i < geometryCount; i++)
                tin.Geometries.Add(Read<Triangle>());

            return tin;
        }

        private Triangle ReadTriangle(Dimension dimension)
        {
            Triangle triangle = new Triangle();

            uint ringCount = wkbReader.ReadUInt32();

            if (ringCount > 0)
            {
                uint exteriorRingCount = wkbReader.ReadUInt32();
                for (int i = 0; i < exteriorRingCount; i++)
                    triangle.ExteriorRing.Points.Add(ReadPoint(dimension));

                for (int i = 1; i < ringCount; i++)
                {
                    triangle.InteriorRings.Add(new LinearRing());

                    uint interiorRingCount = wkbReader.ReadUInt32();
                    for (int j = 0; j < interiorRingCount; j++)
                        triangle.InteriorRings[i - 1].Points.Add(ReadPoint(dimension));
                }
            }

            return triangle;
        }
    }
}
