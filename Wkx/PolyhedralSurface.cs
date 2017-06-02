using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class PolyhedralSurface : Surface, IEquatable<PolyhedralSurface>
    {
        public override GeometryType GeometryType { get { return GeometryType.PolyhedralSurface; } }
        
        public PolyhedralSurface()
            : base()
        {
        }

        public PolyhedralSurface(IEnumerable<Polygon> geometries)
            : base(geometries)
        {
        }

        public bool Equals(PolyhedralSurface other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }        
    }
}
