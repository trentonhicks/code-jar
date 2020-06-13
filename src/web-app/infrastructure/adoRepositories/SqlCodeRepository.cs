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

        public async Task AddCodesAsync(IEnumerable<GeneratedCode> codes)
        {
            try
            {
                await _connection.OpenAsync();

                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = $@"INSERT INTO Codes (BatchID, SeedValue, State) VALUES (@batchID, @seedValue, 0)";

                    command.Parameters.AddWithValue("@batchID", null);
                    command.Parameters.AddWithValue("@seedValue", null);

                    foreach(var code in codes)
                    {
                        command.Parameters["@batchID"].Value = code.BatchId;
                        command.Parameters["@seedValue"].Value = code.SeedValue;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<CodeDto> GetCodeAsync(int value)
        {
            try
            {
                await _connection.OpenAsync();

                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue FROM Codes
                                            WHERE Codes.SeedValue = @seedValue";

                    command.Parameters.AddWithValue("@seedValue", value);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            var code = new CodeDto();
                            code.Id = (int) reader["ID"];
                            code.State = CodeStateSerializer.DeserializeState((byte) reader["State"]);
                            code.SeedValue = (int) reader["SeedValue"];
                            return code;
                        }
                    }
                }

                return new CodeDto();
            }
            
            finally
            {
                await _connection.CloseAsync();
            }
            
        }

        public async Task<List<CodeDto>> GetCodesAsync(Guid batchID, int pageNumber, int pageSize)
        {
            try
            {
                await _connection.OpenAsync();

                var codes = new List<CodeDto>();

                var p = PageHelper.PaginationPageNumber(pageNumber, pageSize);

                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = @"SELECT Codes.ID, Codes.SeedValue, Codes.State FROM Codes
                                            WHERE Codes.BatchId = @batchID
                                            ORDER BY ID OFFSET @page ROWS FETCH NEXT @pageSize ROWS ONLY";

                    command.Parameters.AddWithValue("@page", p);
                    command.Parameters.AddWithValue("@pageSize", pageSize);
                    command.Parameters.AddWithValue("@batchID", batchID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var code = new CodeDto();

                            code.Id = (int) reader["ID"];
                            code.SeedValue = (int)reader["SeedValue"];
                            code.State = CodeStateSerializer.DeserializeState((byte) reader["State"]);

                            codes.Add(code);
                        }
                    }
                }

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
                    command.CommandText = $@"UPDATE Codes
                                             SET State = @state,
                                             SET DeactivatedBy = @deactivatedBy,
                                             SET DeactivatedOn = @deactivatedOn,
                                             SET RedeemedBy = @redeemedBy,
                                             SET RedeemedOn = @redeemedOn
                                             WHERE ID = @id";

                    var deactivatedBy = code is DeactivateCode ? ((DeactivateCode)code).By : null;
                    var deactivatedOn = code is DeactivateCode ? ((DeactivateCode)code).When : null;
                    var redeemedBy = code is RedeemCode ? ((RedeemCode)code).By : null;
                    var redeemedOn = code is RedeemCode ? ((RedeemCode)code).When : null;

                    command.Parameters.AddWithValue("@state", CodeStateSerializer.SerializeState(code.State));
                    command.Parameters.AddWithValue("@deactivatedBy", deactivatedBy);
                    command.Parameters.AddWithValue("@deactivatedOn", deactivatedOn);
                    command.Parameters.AddWithValue("@redeemedBy", redeemedBy);
                    command.Parameters.AddWithValue("@redeemedOn", redeemedOn);
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
                    command.CommandText = @"SELECT Codes.ID, Codes.State FROM Codes
                                            WHERE Codes.SeedValue = @seedValue AND Codes.State = 1";

                    command.Parameters.AddWithValue("@seedValue", value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var code = new RedeemCode
                            {
                                Id = (int) reader["ID"]
                            };

                            code.GetType()
                            .GetProperty(nameof(code.State))
                            .SetValue(code, CodeStateSerializer.DeserializeState((byte) reader["State"]));

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
                    command.CommandText = @"SELECT Codes.ID, Codes.State FROM Codes
                                            WHERE Codes.SeedValue = @seedValue AND (Codes.State = 0 OR Codes.State = 1)";
                    command.Parameters.AddWithValue("@seedValue", value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var code = new DeactivateCode
                            {
                                Id = (int) reader["Id"]
                            };

                            code.GetType()
                            .GetProperty(nameof(code.State))
                            .SetValue(code, CodeStateSerializer.DeserializeState((byte) reader["State"]));

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
                    command.CommandText = @"SELECT Codes.ID, Codes.State FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE Codes.State = 0 AND Batch.DateActive <= @date";

                    command.Parameters.AddWithValue("@date", date);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            var code = new ActivateCode();
                            code.Id = (int) reader["ID"];

                            code.GetType()
                            .GetProperty(nameof(code.State))
                            .SetValue(code, CodeStateSerializer.DeserializeState((byte) reader["State"]));

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
                    command.CommandText = @"SELECT Codes.ID, Codes.State FROM Codes
                                            INNER JOIN Batch ON Batch.ID = Codes.BatchID
                                            WHERE (Codes.State = 0 OR Codes.State = 1) AND Batch.DateExpires <= @forDate";

                    command.Parameters.AddWithValue("@forDate", date.Date);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            var code = new ExpireCode();
                            code.Id = (int) reader["ID"];
                            code.GetType()
                            .GetProperty(nameof(code.State))
                            .SetValue(code, CodeStateSerializer.DeserializeState((byte) reader["State"]));

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