using System;
using System.Collections.Generic;
using System.Linq;

namespace Wkx
{
    public class MultiLineString : Geometry, IEquatable<MultiLineString>
    {
        public override GeometryType GeometryType { get { return GeometryType.MultiLineString; } }
        public override bool IsEmpty { get { return !LineStrings.Any(); } }

        public List<LineString> LineStrings { get; private set; }

        public MultiLineString()
            : this(new List<LineString>())
        {
        }

        public MultiLineString(IEnumerable<LineString> lineStrings)
        {
            LineStrings = new List<LineString>(lineStrings);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MultiLineString))
                return false;

            return Equals((MultiLineString)obj);
        }

        public bool Equals(MultiLineString other)
        {
            return LineStrings.SequenceEqual(other.LineStrings);
        }

        public override int GetHashCode()
        {
            return new { LineStrings }.GetHashCode();
        }
    }
}
