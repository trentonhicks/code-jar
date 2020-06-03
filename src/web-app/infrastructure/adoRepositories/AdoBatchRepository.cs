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

        public async Task AddBatchAsync(Batch batch)
        {
            await _connection.OpenAsync();

            var command = _connection.CreateCommand();
                command.CommandText = @"
                DECLARE @offsetStart int
                SET @offsetStart = (SELECT ISNULL(MAX(OffsetEnd) + 1, 0) FROM Batch)

                INSERT INTO Batch (BatchName, OffsetStart, BatchSize, DateActive, DateExpires, State)
                VALUES(@batchName, @offsetStart, @batchSize, @dateActive, @dateExpires, @state)
                SELECT SCOPE_IDENTITY()";

                command.Parameters.AddWithValue("@batchName", batch.BatchName);
                command.Parameters.AddWithValue("@batchSize", batch.BatchSize);
                command.Parameters.AddWithValue("@dateActive", batch.DateActive);
                command.Parameters.AddWithValue("@dateExpires", batch.DateExpires);
                command.Parameters.AddWithValue("@state", BatchStates.ConvertToByte(batch.State));
                batch.ID = Convert.ToInt32(await command.ExecuteScalarAsync());

            await _connection.CloseAsync();
        }

        public async Task<Batch> GetBatchAsync(int id)
        {
            var batch = new Batch();
            
            await _connection.OpenAsync();

            var command = _connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Batch WHERE ID = @id";

                command.Parameters.AddWithValue("@id", id);
                using(var reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        batch.ID = id;
                        batch.BatchName = (string)reader["BatchName"];
                        batch.BatchSize = (int)reader["BatchSize"];
                        batch.OffsetStart = (int)reader["OffsetStart"];
                        batch.OffsetEnd = (int)reader["OffsetEnd"];
                        batch.DateActive = (DateTime)reader["DateActive"];
                        batch.DateExpires = (DateTime)reader["DateExpires"];
                        batch.State = BatchStates.ConvertToString((byte)reader["State"]);
                    }
                }

            await _connection.CloseAsync();

            return batch;
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
                            BatchSize = (int)reader["BatchSize"],
                            DateActive = (DateTime)reader["DateActive"],
                            DateExpires = (DateTime)reader["DateExpires"],
                            State = BatchStates.ConvertToString((byte)reader["State"])
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