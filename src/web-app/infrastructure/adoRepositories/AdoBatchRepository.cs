using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CodeJar.Domain;

namespace CodeJar.Infrastructure
{
    public class AdoBatchRepository : IBatchRepository
    {
        private SqlConnection _connection;

        public AdoBatchRepository(SqlConnection connection)
        {
            _connection = connection;
        }
        public async Task<List<Batch>> GetBatchesAsync()
        {
            // Create variable to store batches
            var batches = new List<Batch>();

            await _connection.OpenAsync();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Batch";

                // Get all the batches
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var batch = new Batch
                        {
                            ID = (int)reader["ID"],
                            BatchName = (string)reader["BatchName"],
                            CodeIDStart = (int)reader["CodeIDStart"],
                            CodeIDEnd = (int)reader["CodeIDEnd"],
                            BatchSize = (int)reader["BatchSize"],
                            DateActive = (DateTime)reader["DateActive"],
                            DateExpires = (DateTime)reader["DateExpires"],
                        };

                        // Add each batch to the list of batches
                        batches.Add(batch);
                    }
                }
            }

            await _connection.CloseAsync();

            return batches;
        }
    }
}