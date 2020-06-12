using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using CodeJar.Domain;

namespace CodeJar.Infrastructure
{
    public class SqlCodeRepository : ICodeRepository
    {
        private readonly SqlConnection _connection;

        public SqlCodeRepository(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task AddCodesAsync(IEnumerable<Code> codes)
        {
            try
            {
                await _connection.OpenAsync();

                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = $@"INSERT INTO Codes (BatchID, SeedValue, State) VALUES (@batchID, @seedValue, 0)";

                    command.Parameters.AddWithValue("@seedValue", null);
                    command.Parameters.AddWithValue("@stateGenerated", null);
                    command.Parameters.AddWithValue("@batchID", null);

                    foreach(var code in codes)
                    {
                        command.Parameters["@seedValue"].Value = code.SeedValue;
                        command.Parameters["@stateGenerated"].Value = CodeStateSerializer.SerializeState(code.State);
                        command.Parameters["@batchID"].Value = code.BatchId;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<Code> GetCodeAsync(int value)
        {
            try
            {
                await _connection.OpenAsync();

                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE Codes.SeedValue = @seedValue";

                    command.Parameters.AddWithValue("@seedValue", value);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                            var code = new ExpireCode();
                            code.Id = (int) reader["ID"];
                            code.SeedValue = (int)reader["SeedValue"];
                            code.BatchId = (Guid) reader["BatchId"];
                            code.DateActive = (DateTime) reader["DateActive"];
                            code.DateExpires = (DateTime) reader["DateExpires"];

                            return code;
                        }
                    }
                }

                return new Code();
            }
            
            finally
            {
                await _connection.CloseAsync();
            }
            
        }

        public async Task<List<Code>> GetCodesAsync(Guid batchID, int pageNumber, int pageSize)
        {
            try
            {
                await _connection.OpenAsync();

                // Create list to store codes gathered from the database
                var codes = new List<Code>();

                var p = PageHelper.PaginationPageNumber(pageNumber, pageSize);

                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE Codes.BatchId = @batchID
                                            ORDER BY ID OFFSET @page ROWS FETCH NEXT @pageSize ROWS ONLY";

                    command.Parameters.AddWithValue("@page", p);
                    command.Parameters.AddWithValue("@pageSize", pageSize);
                    command.Parameters.AddWithValue("@batchID", batchID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var code = new Code();

                            code.Id = (int) reader["ID"];
                            code.SeedValue = (int)reader["SeedValue"];
                            code.BatchId = (Guid) reader["BatchId"];
                            code.DateActive = (DateTime) reader["DateActive"];
                            code.DateExpires = (DateTime) reader["DateExpires"];
                            code.State = CodeStateSerializer.DeserializeState((byte) reader["State"]);

                            // Add code to the list
                            codes.Add(code);
                        }
                    }
                }

                // Return the list of codes
                return codes;
            }

            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task UpdateCodesAsync(List<Code> codes)
        {
            try
            {
                await _connection.OpenAsync();
                
                using(var command = _connection.CreateCommand())
                {
                    foreach(var code in codes)
                    {
                        command.Parameters.Clear();
                        command.CommandText = $@"UPDATE Codes SET State = @state WHERE ID = @id";

                        // Insert values
                        command.Parameters.AddWithValue("@state", CodeStateSerializer.SerializeState(code.State));
                        command.Parameters.AddWithValue("@id", code.Id);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }

            finally
            {
                await _connection.CloseAsync();
            }
        }
        
        public async Task UpdateCodeAsync(Code code)
        {
            try
            {
                await _connection.OpenAsync();
                
                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = $@"UPDATE Codes SET State = @state WHERE ID = @id";

                    // Insert values
                    command.Parameters.AddWithValue("@state", CodeStateSerializer.SerializeState(code.State));
                    command.Parameters.AddWithValue("@id", code.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }

            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<RedeemCode> GetCodeForRedemptionAsync(int value)
        {
            try
            {
                await _connection.OpenAsync();
                
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE Codes.SeedValue = @seedValue AND Codes.State = 1";

                    command.Parameters.AddWithValue("@seedValue", value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var code = new RedeemCode
                            {
                                BatchId = (Guid) reader["BatchId"],
                                DateActive = (DateTime) reader["DateActive"],
                                DateExpires = (DateTime) reader["DateExpires"],
                                Id = (int) reader["ID"],
                                SeedValue = (int) reader["SeedValue"],
                                State = CodeStateSerializer.DeserializeState((byte) reader["State"]),
                            };

                            return code;
                        }
                    }
                }

                return new RedeemCode();
            }

            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<DeactivateCode> GetCodeForDeactivationAsync(int value)
        {
            try
            {
                await _connection.OpenAsync();

                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE Codes.SeedValue = @seedValue AND (Codes.State = 0 OR Codes.State = 1)";
                    command.Parameters.AddWithValue("@seedValue", value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var code = new DeactivateCode
                            {
                                BatchId = (Guid) reader["BatchId"],
                                DateActive = (DateTime) reader["DateActive"],
                                DateExpires = (DateTime) reader["DateExpires"],
                                Id = (int) reader["Id"],
                                SeedValue = (int) reader["SeedValue"],
                                State = CodeStateSerializer.DeserializeState((byte) reader["State"]),
                            };

                            return code;
                        }
                    }
                }

                return new DeactivateCode();
            }

            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async IAsyncEnumerable<ActivateCode> GetCodesForActivationAsync(DateTime date)
        {
            try
            {
                await _connection.OpenAsync();

                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE Codes.State = 0 AND Batch.DateActive <= @date";

                    command.Parameters.AddWithValue("@date", date);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                            var code = new ActivateCode();
                            code.Id = (int) reader["ID"];
                            code.SeedValue = (int)reader["SeedValue"];
                            code.BatchId = (Guid) reader["BatchId"];
                            code.DateActive = (DateTime) reader["DateActive"];
                            code.DateExpires = (DateTime) reader["DateExpires"];

                            yield return code;
                        }
                    }
                }
            }

            finally
            {
                await _connection.CloseAsync();
            }

        }

        public async IAsyncEnumerable<ExpireCode> GetCodesForExpirationAsync(DateTime date)
        {
            try
            {
                await _connection.OpenAsync();

                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE (Codes.State = 0 OR Codes.State = 1) AND Batch.DateExpires <= @forDate";

                    command.Parameters.AddWithValue("@forDate", date.Date);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                            var code = new ExpireCode();
                            code.Id = (int) reader["ID"];
                            code.SeedValue = (int)reader["SeedValue"];
                            code.BatchId = (Guid) reader["BatchId"];
                            code.DateActive = (DateTime) reader["DateActive"];
                            code.DateExpires = (DateTime) reader["DateExpires"];

                            yield return code;
                        }
                    }
                }
            }

            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}