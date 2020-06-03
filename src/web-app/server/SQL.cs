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
            FilePath = filePath;
        }

        // SQL connection string
        public SqlConnection Connection { get; set; }
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the next seed value that will be used to generate codes
        /// </summary>
        /// <returns></returns>
      

        public Code GetCode(string stringValue, string alphabet)
        {
            var seedValue = CodeConverter.ConvertFromCode(stringValue, alphabet);
            var code = new Code();

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Codes WHERE SeedValue = @seedValue";
                command.Parameters.AddWithValue("@seedValue", seedValue);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var seed = (int)reader["SeedValue"];
                        code.State = (byte)reader["State"];
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
        public List<Code> GetCodes(int batchID, int pageNumber, string alphabet, int pageSize)
        {
            // Create list to store codes gathered from the database
            var codes = new List<Code>();

            Connection.Open();

            var p = Pagination.PaginationPageNumber(pageNumber, pageSize);

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"
                                        SELECT * FROM Codes WHERE BatchID = @batchID
                                        ORDER BY ID OFFSET @page ROWS FETCH NEXT @pageSize ROWS ONLY";

                command.Parameters.AddWithValue("@page", p);
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@batchID", batchID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        // Store code in a variable
                        var code = new Code();

                        //Stores SeedValue outside of code object
                        var seed = (int)reader["SeedValue"];

                        code.State = (byte)reader["State"];
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);

                        // Add code to the list
                        codes.Add(code);
                    }
                }
            }

            Connection.Close();

            // Return the list of codes
            return codes;
        }

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

                command.Parameters.AddWithValue("@inactive", CodeStates.Inactive);
                command.Parameters.AddWithValue("@active", CodeStates.Active);
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

                command.Parameters.AddWithValue("@inactive", CodeStates.Inactive);
                command.Parameters.AddWithValue("@active", CodeStates.Active);
                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        public int CheckIfCodeCanBeRedeemed(string code, string alphabet)
        {
            var seedvalue = CodeConverter.ConvertFromCode(code, alphabet);
            int codeID = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = @redeemed
                                        OUTPUT INSERTED.ID
                                        WHERE SeedValue = @seedvalue AND [State] = @active";

                command.Parameters.AddWithValue("@redeemed", CodeStates.Redeemed);
                command.Parameters.AddWithValue("@active", CodeStates.Active);
                command.Parameters.AddWithValue("@seedvalue", seedvalue);

                codeID = (int)command.ExecuteScalar();
            }

            Connection.Close();

            if (codeID != 0)
            {
                return codeID;
            }
            return -1;
        }
    }
}