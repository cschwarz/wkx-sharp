using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public abstract class MultiCurve<T> : GeometryCollection<T>, IEquatable<MultiCurve<T>> where T : Curve
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

        public override Geometry CurveToLine(double tolerance)
        {
            return new MultiLineString(Geometries.Select(g => g.CurveToLine(tolerance) as LineString));
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
