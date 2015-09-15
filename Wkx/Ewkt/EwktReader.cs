using System.Text.RegularExpressions;

namespace Wkx
{
    internal class EwktReader : WktReader
    {
        internal EwktReader(string value)
            : base(value)
        {
        }

        internal override Geometry Read()
        {
            Match match = MatchRegex(@"^SRID=(\d+);");

            Geometry geometry = base.Read();
            
            if (match.Success)
                geometry.Srid = int.Parse(match.Groups[1].Value);

            return geometry;
        }
    }
}
