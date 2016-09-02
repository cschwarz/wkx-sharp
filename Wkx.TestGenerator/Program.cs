using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wkx.TestGenerator
{
    class Program
    {
        static IEnumerable<GeometryFormat> geometryFormats = new List<GeometryFormat>()
        {
            new GeometryFormat("wkt", "ST_AsText", "ST_GeomFromText", false),
            new GeometryFormat("ewkt", "ST_AsEWKT", "ST_GeomFromEWKT", false, 4326),
            new GeometryFormat("wkb", "ST_AsBinary", "ST_GeomFromWKB", true),
            new GeometryFormat("ewkb", "ST_AsEWKB", "ST_GeomFromEWKB", true, 4326),
            new GeometryFormat("wkbXdr", "ST_AsBinary", "ST_GeomFromWKB", true, null, "'xdr'"),
            new GeometryFormat("ewkbXdr", "ST_AsEWKB", "ST_GeomFromEWKB", true, 4326, "'xdr'")
            //new GeometryFormat("twkb", "ST_AsTWKB", "ST_GeomFromTWKB", true),
            //new GeometryFormat("geojson", "ST_AsGeoJSON", "ST_GeomFromGeoJSON", false)
        };

        static void Main(string[] args)
        {
            JObject testInputData = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("tests.input.json"));
            JObject testOutputData = new JObject();

            using (NpgsqlConnection connection = new NpgsqlConnection("Server=localhost;Port=5432;User Id=postgres;Password=postgres"))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    foreach (var testDimension in testInputData)
                    {
                        JObject dimension = new JObject();
                        testOutputData[testDimension.Key] = dimension;

                        foreach (var testInput in testDimension.Value.ToObject<JObject>())
                        {
                            command.CommandText = GenerateTestSql(geometryFormats);

                            command.Parameters.Clear();
                            command.Parameters.Add(new NpgsqlParameter("input", testInput.Value.Value<string>()));

                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                reader.Read();

                                JObject testOutput = new JObject();
                                JObject testOutputResult = new JObject();

                                dimension[testInput.Key] = testOutput;

                                foreach (GeometryFormat geometryFormat in geometryFormats)
                                {
                                    testOutput[geometryFormat.Name] = reader.GetString(reader.GetOrdinal(geometryFormat.Name));

                                    string result = reader.GetString(reader.GetOrdinal(geometryFormat.Name + "result"));

                                    if (testInput.Value.Value<string>() != result)
                                        testOutputResult[geometryFormat.Name] = result;
                                }

                                if (testOutputResult.Count > 0)
                                    testOutput["results"] = testOutputResult;
                            }
                        }
                    }
                }
            }

            File.WriteAllText("testdata.json", JsonConvert.SerializeObject(testOutputData, Formatting.Indented));
        }

        private static string GenerateTestSql(IEnumerable<GeometryFormat> geometryFormats)
        {
            return "SELECT " + string.Join(", \r\n", geometryFormats.Select(t => t.GenerateSql()));
        }
    }
}
