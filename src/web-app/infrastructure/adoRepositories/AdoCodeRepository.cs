using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using CodeJar.Domain;

namespace CodeJar.Infrastructure
{
    public class AdoCodeRepository : ICodeRepository
    {
        private readonly SqlConnection _connection;

        private string FilePath {get; set;}
        public AdoCodeRepository(SqlConnection connection)
        {
            FilePath = "C:\\Binary.bin";
            _connection = connection;
        }
        public async Task AddCodesAsync(Batch batch)
        {
            await _connection.OpenAsync();

            using(var command = _connection.CreateCommand())
            {
                using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
                {
                // Get the next offset position
                
                    if (batch.OffsetStart % 4 != 0)
                    {
                        throw new ArgumentException("Offset must be divisible by 4");
                    }

                    // Loop to the last offset position
                    for (var i = batch.OffsetStart; i < batch.OffsetEnd; i += 4)
                    {
                        // Set reader to offset position
                        reader.BaseStream.Position = i;
                        var seedvalue = reader.ReadInt32();

                        // Insert code
                        command.Parameters.Clear();
                        command.CommandText = $@"INSERT INTO Codes (BatchID, SeedValue, State) VALUES (@batchID, @Seedvalue, @StateGenerated)";

                        // Insert values
                        command.Parameters.AddWithValue("@Seedvalue", seedvalue);
                        command.Parameters.AddWithValue("@StateGenerated", States.Generated);
                        command.Parameters.AddWithValue("@batchID", batch.ID);
                        await command.ExecuteNonQueryAsync();

                        // Update code to active state if dateActive is today
                        if (batch.DateActive.Day == DateTime.Now.Day)
                        {
                            command.CommandText = "UPDATE Codes SET State = @StateActive WHERE SeedValue = @Seedvalue";
                            command.Parameters.AddWithValue("@StateActive", States.Active);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            
            await _connection.CloseAsync();
        }
    }
}