using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class CurvePolygon : Surface, IEquatable<CurvePolygon>
    {
        public override GeometryType GeometryType { get { return GeometryType.CurvePolygon; } }
        
        public CurvePolygon()
            : base()
        {
        }

        public CurvePolygon(IEnumerable<Geometry> geometries)
            : base(geometries)
        {
        }

        public bool Equals(CurvePolygon other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }        
    }
}
