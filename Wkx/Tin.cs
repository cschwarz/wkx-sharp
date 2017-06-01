using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class Tin : PolyhedralSurface, IEquatable<Tin>
    {
        public override GeometryType GeometryType { get { return GeometryType.Tin; } }
        
        public Tin()            
        {
        }

        public Tin(IEnumerable<Geometry> geometries)
            : base(geometries)
        {
        }
        
        public bool Equals(Tin other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }        
    }
}
