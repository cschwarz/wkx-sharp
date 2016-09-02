namespace Wkx.TestGenerator
{
    class GeometryFormat
    {
        public string Name { get; private set; }
        public string SqlAsFunction { get; private set; }
        public string SqlFromFunction { get; private set; }
        public bool IsBinary { get; private set; }
        public int? Srid { get; set; }
        public string SqlAdditionalFlags { get; private set; }

        public GeometryFormat(string name, string sqlAsFunction, string sqlFromFunction, bool isBinary, int? srid = null, string sqlAdditionalFlags = null)
        {
            Name = name;
            SqlAsFunction = sqlAsFunction;
            SqlFromFunction = sqlFromFunction;
            IsBinary = isBinary;
            Srid = srid;
            SqlAdditionalFlags = sqlAdditionalFlags;
        }

        public string GenerateSql()
        {
            string targetSql = string.Format("{0}(ST_GeomFromText(@input{1}){2})", SqlAsFunction,
                Srid.HasValue ? string.Concat(", ", Srid.Value.ToString()) : string.Empty,
                string.IsNullOrEmpty(SqlAdditionalFlags) ? string.Empty : string.Concat(", ", SqlAdditionalFlags));

            string targetAsText = string.Format("ST_AsText({0}({1}))", SqlFromFunction, targetSql);

            if (IsBinary)
                targetSql = string.Format("encode({0}, 'hex')", targetSql);

            return string.Format("{0} {1}, {2} {1}result", targetSql, Name, targetAsText);
        }
    }
}
