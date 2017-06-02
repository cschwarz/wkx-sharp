using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiCurve : GeometryCollection<Curve>, IEquatable<MultiCurve>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiCurve; } }

        public MultiCurve()
            : base()
        {
        }

        public MultiCurve(IEnumerable<Curve> geometries)
            : base(geometries)
        {
        }

        public bool Equals(MultiCurve other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }
    }
}
