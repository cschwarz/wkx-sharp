using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Collections.Generic;
using System.IO;

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
            new GeometryFormat("ewkbXdr", "ST_AsEWKB", "ST_GeomFromEWKB", true, 4326, "'xdr'"),
            new GeometryFormat("ewkbNoSrid", "ST_AsEWKB", "ST_GeomFromEWKB", true),
            new GeometryFormat("ewkbXdrNoSrid", "ST_AsEWKB", "ST_GeomFromEWKB", true, null, "'xdr'")

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
                            JObject testOutput = new JObject();
                            JObject testOutputResult = new JObject();

                            dimension[testInput.Key] = testOutput;

                            foreach (GeometryFormat geometryFormat in geometryFormats)
                            {
                                command.CommandText = GenerateTestSql(geometryFormat);

                                command.Parameters.Clear();
                                command.Parameters.Add(new NpgsqlParameter("input", testInput.Value.Value<string>()));

                                try
                                {
                                    using (NpgsqlDataReader reader = command.ExecuteReader())
                                    {
                                        reader.Read();
                                        
                                        testOutput[geometryFormat.Name] = reader.GetString(reader.GetOrdinal(geometryFormat.Name));

                                        string result = reader.GetString(reader.GetOrdinal(geometryFormat.Name + "result"));

                                        if (testInput.Value.Value<string>() != result)
                                            testOutputResult[geometryFormat.Name] = result;
                                    }
                                }
                                catch (PostgresException)
                                {
                                    testOutput[geometryFormat.Name] = null;
                                }
                            }

                            if (testOutputResult.Count > 0)
                                testOutput["results"] = testOutputResult;
                        }
                    }
                }
            }

            File.WriteAllText("../Wkx.Tests/testdata.json", JsonConvert.SerializeObject(testOutputData, Formatting.Indented));
        }

        private static string GenerateTestSql(GeometryFormat geometryFormat)
        {
            return "SELECT " + geometryFormat.GenerateSql();
        }
    }
}
