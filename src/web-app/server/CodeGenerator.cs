using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
namespace CodeJar.WebApp
{
    public class CodeGenerator
    {
        public CodeGenerator(string connectionString, string filePath)
        {
            ConnectionString = connectionString;
            FilePath = filePath;
        }
        public string ConnectionString { get; set; }
        public string FilePath {get; set;}

        
        public void CreateDigitalCode(int batchSize, DateTime dateActive, DateTime dateExpires, SqlCommand command)
        {
            var sql = new SQL(ConnectionString);

                // Loop through number of codes to generate
                using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
                {

                    // Get the next offset position
                    var firstAndLastOffset = sql.GetOffset(command, batchSize);
                    if (firstAndLastOffset[0] % 4 != 0)
                    {
                        throw new ArgumentException("Offset must be divisible by 4");
                    }

                    // Loop to the last offset position
                    for(var i = firstAndLastOffset[0]; i < firstAndLastOffset[1]; i+= 4)
                    {
                        // Set reader to offset position
                        reader.BaseStream.Position = i;
                        var seedvalue = reader.ReadInt32();

                        // Insert code
                        command.Parameters.Clear();
                        command.CommandText = $@"INSERT INTO Codes (SeedValue, State) VALUES (@Seedvalue, @StateGenerated)";

                        // Insert values
                        command.Parameters.AddWithValue("@Seedvalue", seedvalue);
                        command.Parameters.AddWithValue("@StateGenerated", States.Generated);
                        command.ExecuteNonQuery();

                        // Update code to active state if dateActive is today
                        if(dateActive.Day == DateTime.Now.Day)
                        {
                            command.CommandText = "UPDATE Codes SET State = @StateActive WHERE SeedValue = @Seedvalue";
                            command.Parameters.AddWithValue("@StateActive", States.Active);
                            command.ExecuteNonQuery();
                        }               
                    }
                }
            
        }
    }
}
