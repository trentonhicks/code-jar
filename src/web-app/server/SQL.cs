using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using CodeJar.Domain;
using CodeJar.Infrastructure;

namespace CodeJar.WebApp
{

    public class SQL
    {
        public SQL(string connectionString, string filePath)
        {
            Connection = new SqlConnection(connectionString);
        }

        // SQL connection string
        public SqlConnection Connection { get; set; }

        /// <summary>
        /// Gets the next seed value that will be used to generate codes
        /// </summary>
        /// <returns></returns>
        public Code GetCode(string stringValue, string alphabet)
        {
            Code code = null;
            var seedValue = CodeConverter.ConvertFromCode(stringValue, alphabet);

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Codes WHERE SeedValue = @seedValue";
                command.Parameters.AddWithValue("@seedValue", seedValue);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);

                        code = new Code(state);
                        var seed = (int)reader["SeedValue"];
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);
                    }
                }
            }

            Connection.Close();

            return code;
        }

        /// <summary>
        /// Returns a list of all the codes from the database
        /// </summary>
        /// <returns></returns>
       
        public int PageCount(int id)
        {
            var pages = 0;

            var pagesRemainder = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT BatchSize FROM Batch WHERE ID = @id";

                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var numberOfCodes = (int)reader["BatchSize"];

                        pages = numberOfCodes / 10;

                        pagesRemainder = numberOfCodes % 10;

                        if (pagesRemainder > 0)
                        {
                            pages++;
                        }
                    }
                }
            }

            Connection.Close();

            return pages;
        }
        public void DeactivateCode(string code, string alphabet)
        {
            var seedvalue = CodeConverter.ConvertFromCode(code, alphabet);

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = @inactive
                                        WHERE SeedValue = @seedvalue AND [State] = @active";

                command.Parameters.AddWithValue("@inactive", CodeStateSerializer.Inactive);
                command.Parameters.AddWithValue("@active", CodeStateSerializer.Active);
                command.Parameters.AddWithValue("@seedvalue", seedvalue);
                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        public void DeactivateBatch(Batch batch)
        {
            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = @inactive
                                        WHERE ID BETWEEN @codeIDStart AND @codeIDEnd AND [State] = @active";

                command.Parameters.AddWithValue("@inactive", CodeStateSerializer.Inactive);
                command.Parameters.AddWithValue("@active", CodeStateSerializer.Active);
                command.ExecuteNonQuery();
            }

            Connection.Close();
        }
    }
}