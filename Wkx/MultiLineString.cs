using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiLineString : MultiCurve<LineString>, IEquatable<MultiLineString>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiLineString; } }

        public MultiLineString()
            : base()
        {
        }

        public MultiLineString(IEnumerable<LineString> lineStrings)
            : base(lineStrings)
        {
        }
        
        public bool Equals(MultiLineString other)
        {
            return Geometries.SequenceEqual(other.Geometries);
        }        
    }
}
