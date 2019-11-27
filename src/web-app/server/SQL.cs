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
        public static SqlConnection Connection { get; set; }

        /// <summary>
        /// Stores codes in the database
        /// </summary>
        /// <param name="code"></param>
        /// <param name="seedvalue"></param>
        public void StoreRequestedCodes(string code, long seedvalue)
        {

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $@"INSERT INTO [6 Digit Code] (Seedvalue, StringValue, State, DateActive, DateExpires) VALUES (@Seedvalue, @StringValue, @State, @DateActive, @DateExpires)";

                // Insert values
                command.Parameters.AddWithValue("@Seedvalue", seedvalue);
                command.Parameters.AddWithValue("@StringValue", code);
                command.Parameters.AddWithValue("@State", "Active");
                command.Parameters.AddWithValue("@DateActive", DateTime.Now);
                command.Parameters.AddWithValue("@DateExpires", DateTime.Today.AddDays(8));

                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        /// <summary>
        /// Gets the next seed value that will be used to generate codes
        /// </summary>
        /// <returns></returns>
        public long GetSeedValue()
        {
            long seedValue = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT TOP 1 SeedValue FROM [6 Digit Code] ORDER BY ID DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        seedValue = (long)reader["SeedValue"];

                        //If the seed value is not equal to 0 add 4 to the previous seed value to keep the codes unique 
                        if (seedValue != 0)
                        {
                            seedValue += 4;
                        }
                    }
                }
                command.ExecuteNonQuery();
            }

            Connection.Close();
            return seedValue;
        }

        /// <summary>
        /// Returns a list of all the codes from the database
        /// </summary>
        /// <returns></returns>
        public List<Code> GetCodes()
        {
            // Create list to store codes gathered from the database
            var codes = new List<Code>();

            Connection.Open();

            using(var command = Connection.CreateCommand())
            {
                // Select all codes from the database
                command.CommandText = "SELECT * FROM [6 Digit Code]";

                // Read all the rows
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        // Store code in a variable
                        var code = new Code()
                        {
                            ID = (int)reader["ID"],
                            SeedValue = (long)reader["SeedValue"],
                            StringValue = (string)reader["StringValue"],
                            State = (string)reader["State"],
                            DateActive = (DateTime)reader["DateActive"],
                            DateExpires = (DateTime)reader["DateExpires"]
                        };

                        // Add code to the list
                        codes.Add(code);
                    }
                }
            }

            Connection.Close();

            // Return the list of codes
            return codes;
        }
    }
}
