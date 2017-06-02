using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public abstract class Surface : Geometry, IEquatable<Surface>
    {
        public override GeometryType GeometryType { get { return GeometryType.Surface; } }
        public override bool IsEmpty { get { return !Geometries.Any(); } }

        public List<Geometry> Geometries { get; private set; }

        public Surface()
            : this(new List<Geometry>())
        {
        }

        public Surface(IEnumerable<Geometry> geometries)
        {
            Geometries = new List<Geometry>(geometries);

            if (Geometries.Any())
                Dimension = Geometries.First().Dimension;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Surface))
                return false;

            return Equals((Surface)obj);
        }

        public bool Equals(Surface other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }

        public override int GetHashCode()
        {
            return new { Geometries }.GetHashCode();
        }

        public override Point GetCenter()
        {
            return Geometries.Select(g => g.GetCenter()).GetCenter();
        }

        public override BoundingBox GetBoundingBox()
        {
            return Geometries.Select(g => g.GetBoundingBox()).GetBoundingBox();
        }
    }    
}
