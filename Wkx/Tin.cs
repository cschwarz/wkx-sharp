using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class Tin : PolyhedralSurface<Triangle>, IEquatable<Tin>
    {
        public override GeometryType GeometryType { get { return GeometryType.Tin; } }
        
        public Tin()            
        {
        }

        public Tin(IEnumerable<Triangle> geometries)
            : base(geometries)
        {
        }
        
        public bool Equals(Tin other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }        
    }
}
