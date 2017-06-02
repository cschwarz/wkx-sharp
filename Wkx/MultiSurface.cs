using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiSurface : GeometryCollection<Surface>, IEquatable<MultiSurface>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiSurface; } }
                
        public MultiSurface()
            : base()
        {
        }

        public MultiSurface(IEnumerable<Surface> geometries)
            : base(geometries)
        {
        }
        
        public bool Equals(MultiSurface other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }        
    }
}
