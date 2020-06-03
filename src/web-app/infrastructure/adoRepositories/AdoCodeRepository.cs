using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using CodeJar.Domain;

namespace CodeJar.Infrastructure
{
    public class AdoCodeRepository : ICodeRepository
    {
        private readonly SqlConnection _connection;

        public AdoCodeRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task AddCodesAsync(IEnumerable<Code> codes)
        {
            await _connection.OpenAsync();

            using(var command = _connection.CreateCommand())
            {
                // Loop to the last offset position
                foreach(var code in codes)
                {
                    command.Parameters.Clear();
                    command.CommandText = $@"INSERT INTO Codes (BatchID, SeedValue, State) VALUES (@batchID, @Seedvalue, @StateGenerated)";

                    // Insert values
                    command.Parameters.AddWithValue("@Seedvalue", code.SeedValue);
                    command.Parameters.AddWithValue("@StateGenerated", code.State);
                    command.Parameters.AddWithValue("@batchID", code.BatchId);

                    await command.ExecuteNonQueryAsync();
                }
            }
            
            await _connection.CloseAsync();
        }
    }
}