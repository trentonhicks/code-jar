using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace CodeJar.WebApp
{
    public class SQL
    {
        public SQL(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        // SQL connection string
        public SqlConnection Connection { get; set; }

        /// <summary>
        /// Stores codes in the database
        /// </summary>
        /// <param name="code"></param>
        /// <param name="offset"></param>
        public void StoreRequestedCodes(int seedValue, long offset)
        {
            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $@"INSERT INTO Codes (SeedValue, State) VALUES (@Seedvalue, @State)";

                // Insert values
                command.Parameters.AddWithValue("@Seedvalue", seedValue);
                command.Parameters.AddWithValue("@State", 1); // TODO: Replace 1 with enum

                command.ExecuteNonQuery();
            }

            Connection.Close();

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $@"UPDATE Offset SET OffsetValue = @Offset WHERE ID = 1";

                // Insert offset
                command.Parameters.AddWithValue("@Offset", offset);

                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        public void CreateBatch(Batch batch)
        {
            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Batch (BatchName, CodeIDStart, CodeIDEnd, DateActive, DateExpires)
                                        VALUES(@batchName, @codeIDStart, @codeIDEnd, @dateActive, @dateExpires)";

                command.Parameters.AddWithValue("@batchName", batch.BatchName);
                command.Parameters.AddWithValue("@codeIDStart", batch.CodeIDStart);
                command.Parameters.AddWithValue("@codeIDEnd", batch.CodeIDEnd);
                command.Parameters.AddWithValue("@dateActive", batch.DateActive);
                command.Parameters.AddWithValue("@dateExpires", batch.DateExpires);

                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        /// <summary>
        /// Gets the next seed value that will be used to generate codes
        /// </summary>
        /// <returns></returns>
        public long GetOffset()
        {
            long seedValue = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Offset";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        seedValue = (long)reader["OffsetValue"];
                    }
                }
            }

            Connection.Close();
            return seedValue;
        }

        /// <summary>
        /// Returns a list of all the codes from the database
        /// </summary>
        /// <returns></returns>
        public List<Code> GetCodes(int page)
        {
            // Create list to store codes gathered from the database
            var codes = new List<Code>();

            Connection.Open();

            page = 0;

            if(page > 0)
            {
                page *= 10;
            }

            using(var command = Connection.CreateCommand())
            {
                // Select all codes from the database
                command.CommandText = "SELECT * FROM Codes ORDER BY ID OFFSET @page ROWS FETCH NEXT 10 ROWS ONLY";

                command.Parameters.AddWithValue("@page", page);

                // Read all the rows
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        // Store code in a variable
                        var code = new Code()
                        {
                            State = (string)reader["State"],
                            DateActive = (DateTime)reader["DateActive"],
                            DateExpires = (DateTime)reader["DateExpires"]
                        };

                        //Stores SeedValue outside of code object
                        var seed = (int)reader["SeedValue"];

                        code.StringValue = ConvertToCode(seed);
                        
                        // Add code to the list
                        codes.Add(code);
                    }
                }
            }

            Connection.Close();

            // Return the list of codes
            return codes;
        }

         public int PageCount()
         {

              var pages = 0;

              var pagesRemainder = 0;

            Connection.Open();

             using (var command = Connection.CreateCommand())
             {

                 command.CommandText = "SELECT COUNT(*) AS Number FROM Codes";

                  using(var reader = command.ExecuteReader())
                  {
                     

                      while(reader.Read())
                      {
                        
                        var numberOfCodes = (int)reader["Number"] ;

                        pages = numberOfCodes / 10;

                        pagesRemainder = numberOfCodes % 10;

                        if(pagesRemainder > 0)
                        {
                            pages++;
                        }
                      }
                  }
             }

             Connection.Close();

             return pages;
         }

        private string ConvertToCode(int seedvalue)
        {
            string alphabet = "2BCD3FGH4JKLMN5PQRST6VWXYZ";

            var result = EncodeToBaseString(seedvalue, alphabet);

            result = result.PadLeft(6, alphabet[0]);

            return result;
        }

        private int ConvertFromCode(string code,string alphabet)
        {
            var result = DecodeFromBaseString(code, alphabet);

            return result;      
        }

        private static string EncodeToBaseString(int seedvalue, string alphabet)
        {
            var encBase = alphabet.Length;

            var digits = "";
            var num = seedvalue;

            if (num == 0)
                return alphabet[0].ToString();

            while (num > 0)
            {
                digits = alphabet[num % encBase] + digits;
                num = num / encBase;
            }

            return digits;
        }
        
        private static int DecodeFromBaseString(string value, string alphabet)
        {
            var result = 0;

            for (int i = 0; i < value.Length; i++)
            {
                var c = value[value.Length - 1 - i];
                var index = alphabet.IndexOf(c);
                var p = index * (int)Math.Pow(alphabet.Length, i);
                
                result = result + p;
            }

            return result;
        }

        public void InactiveStatus(string code, string alphabet)
        {
            var seedvalue = ConvertFromCode(code, alphabet);

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = 'Inactive' WHERE SeedValue = @seedvalue";

                command.Parameters.AddWithValue("@seedvalue", seedvalue);

                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        public Boolean RedeemedStatus(string code, string alphabet)
        {
            var seedvalue = ConvertFromCode(code, alphabet);

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT [State] FROM Codes
                                        WHERE SeedValue = @seedvalue";

                command.Parameters.AddWithValue("@seedvalue", seedvalue);

                // Read the code that matched the query
                using(var reader = command.ExecuteReader())
                {
                    var codeFound = false;
                    while(reader.Read())
                    {
                        codeFound = true;

                        // If code is not redeemable return false
                        if((string)reader["State"] != "Active")
                        {
                            return false;
                        }
                    }

                    // If the code doesn't exist, return false
                    if(!codeFound)
                    {
                        return false;
                    }
                }

                command.CommandText = @"UPDATE Codes SET [State] = 'Redeemed'
                                        WHERE SeedValue = @seedvalue AND [State] = 'Active'";

                command.ExecuteNonQuery();
            }

            Connection.Close();

            return true;
        }

        public int[] GetCodeIDStartAndEnd(int batchSize)
        {
            var codeIDStart = 1;

            // Get the last code generated and add one (this is the next code that will be generated)
            Connection.Open();

            using(var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT TOP 1 ID FROM Codes ORDER BY ID DESC";
                
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        codeIDStart = (int)reader["ID"] + 1;
                    }
                }
            }

            Connection.Close();

            return new int[2] {codeIDStart, codeIDStart + batchSize - 1};
        }
    }
}