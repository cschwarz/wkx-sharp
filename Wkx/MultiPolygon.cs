using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiPolygon : MultiSurface<Polygon>, IEquatable<MultiPolygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiPolygon; } }

        public MultiPolygon()
            : base()
        {
        }

        public MultiPolygon(IEnumerable<Polygon> polygons)
            : base(polygons)
        {            
        }
        
        public bool Equals(MultiPolygon other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }        
    }
}
