using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace _6_Digit_Code_Generator
{
    class SQL
    {
        public SQL(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        // SQL connection string
        public static SqlConnection Connection { get; set; }


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

        public long GetSeedValue()
        {
            long seedValue = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $@"SELECT TOP 1 SeedValue FROM [6 Digit Code] ORDER BY ID DESC";
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
    }
}
