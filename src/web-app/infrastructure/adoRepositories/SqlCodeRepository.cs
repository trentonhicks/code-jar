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
            using(var command = _connection.CreateCommand())
            {
                // Loop to the last offset position
                foreach(var code in codes)
                {
                    command.Parameters.Clear();
                    command.CommandText = $@"INSERT INTO Codes (BatchID, SeedValue, State) VALUES (@batchID, @Seedvalue, @StateGenerated)";

                    // Insert values
                    command.Parameters.AddWithValue("@Seedvalue", code.SeedValue);
                    command.Parameters.AddWithValue("@StateGenerated", CodeStateSerializer.SerializeState(code.State));
                    command.Parameters.AddWithValue("@batchID", code.BatchId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Code>> GetCodesAsync(Guid batchID, int pageNumber, string alphabet, int pageSize)
        {
           // Create list to store codes gathered from the database
            var codes = new List<Code>();

            var p = PageHelper.PaginationPageNumber(pageNumber, pageSize);

            await _connection.OpenAsync();

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
                        var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                        var seed = (int)reader["SeedValue"];

                        var code = new Code(state);

                        code.Id = (Guid) reader["ID"];
                        code.SeedValue = seed;
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);
                        code.BatchId = (Guid) reader["BatchId"];
                        code.DateActive = (DateTime) reader["DateActive"];
                        code.DateExpires = (DateTime) reader["DateExpires"];

                        // Add code to the list
                        codes.Add(code);
                    }
                }
            }

            // Return the list of codes
            return codes;
        }

        public async Task<List<Code>> GetCodesForExpirationAsync(DateTime date, string alphabet)
        {
             var codes = new List<Code>();

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                        INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                        WHERE Codes.State = 0 OR Codes.State = 1 AND Batch.DateExpires <= @forDate";

                command.Parameters.AddWithValue("@forDate", date.Date);

                using(var reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                        var seed = (int)reader["SeedValue"];

                        var code = new Code(state);
                        code.Id = (int) reader["ID"];
                        code.SeedValue = seed;
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);
                        code.BatchId = (Guid) reader["BatchId"];
                        code.DateActive = (DateTime) reader["DateActive"];
                        code.DateExpires = (DateTime) reader["DateExpires"];

                        codes.Add(code);
                    }
                }
            }

            return codes;
        }

        public async Task<List<Code>> GetCodesForActivationAsync(DateTime forDate, string alphabet)
        {
            var codes = new List<Code>();

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.BatchID, Codes.State, Batch.DateActive, Batch.DateExpires FROM Codes
                                        INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                        WHERE Codes.State = 0 AND Batch.DateActive <= @forDate";

                command.Parameters.AddWithValue("@forDate", forDate.Date);

                using(var reader = await command.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                        var seed = (int)reader["SeedValue"];

                        var code = new Code(state);
                        code.Id = (int) reader["ID"];
                        code.SeedValue = seed;
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);
                        code.BatchId = (Guid) reader["BatchId"];
                        code.DateActive = (DateTime) reader["DateActive"];
                        code.DateExpires = (DateTime) reader["DateExpires"];

                        codes.Add(code);
                    }
                }
            }

            return codes;
        }

        public async Task UpdateCodesAsync(List<Code> codes)
        {
            using(var command = _connection.CreateCommand())
            {
                // Loop to the last offset position
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
        
        public async Task UpdateCodeAsync(Code code)
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

        public async Task<Code> FindCodeBySeedValueAsync(string codeString, string alphabet)
        {
            Code code = null;

            var seedvalue = CodeConverter.ConvertFromCode(codeString, alphabet);

            await _connection.OpenAsync();

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = $@"SELECT Codes.*, Batch.DateActive, Batch.DateExpires FROM Codes
                                         INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                         WHERE SeedValue = @seedvalue";

                command.Parameters.AddWithValue("@seedvalue", seedvalue);
                using(var reader = await command.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);

                        code = new Code(state);
                        code.SeedValue = seedvalue;
                        code.StringValue = codeString;
                        code.Id = (int)reader["ID"];
                        code.BatchId = (Guid)reader["BatchID"];
                        code.DateActive = (DateTime)reader["DateActive"];
                        code.DateExpires = (DateTime)reader["DateExpires"];
                    }
                }
            }

            return code;
        }

        public async Task<Code> GetCodeAsync(string stringValue, string alphabet)
        {
            Code code = null;
            var seedValue = CodeConverter.ConvertFromCode(stringValue, alphabet);

            await _connection.OpenAsync();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Codes WHERE SeedValue = @seedValue";
                command.Parameters.AddWithValue("@seedValue", seedValue);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var state = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                        code = new Code(state);
                        var seed = (int)reader["SeedValue"];
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);
                    }
                }
            }

            return code;
        }
    }
}