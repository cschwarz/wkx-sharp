using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiCurve<T> : GeometryCollection<T>, IEquatable<MultiCurve<T>> where T : Curve
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiCurve; } }

        public MultiCurve()
            : base()
        {
        }

        public MultiCurve(IEnumerable<T> geometries)
            : base(geometries)
        {
        }

        public bool Equals(MultiCurve<T> other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }
    }

    public class MultiCurve : MultiCurve<Curve>
    {
        public MultiCurve()
              : base()
        {
        }

        public MultiCurve(IEnumerable<Curve> geometries)
            : base(geometries)
        {
        }
    }
}
