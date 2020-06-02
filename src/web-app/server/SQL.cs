using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using CodeJar.Domain;

namespace CodeJar.WebApp
{

    public class SQL
    {
        public SQL(string connectionString, string filePath)
        {
            Connection = new SqlConnection(connectionString);
            FilePath = filePath;
        }

        // SQL connection string
        public SqlConnection Connection { get; set; }
        public string FilePath { get; set; }


        /// <summary>
        /// Stores codes in the database
        /// </summary>
        /// <param name="code"></param>
        /// <param name="offset"></param>


        public void CreateBatch(Batch batch)
        {
            SqlTransaction transaction;
            Connection.Open();

            // Begin transaction
            transaction = Connection.BeginTransaction();

            // Create command and assiociate it with the transaction
            var command = Connection.CreateCommand();
            command.Transaction = transaction;

            try
            {
                // Create batch            
                command.CommandText = @"
                DECLARE @codeIDStart int
                SET @codeIDStart = (SELECT ISNULL(MAX(CodeIDEnd), 0) FROM Batch) + 1

                INSERT INTO Batch (BatchName, CodeIDStart, BatchSize, DateActive, DateExpires)
                VALUES(@batchName, @codeIDStart, @batchSize, @dateActive, @dateExpires)
                SELECT SCOPE_IDENTITY()";

                command.Parameters.AddWithValue("@batchName", batch.BatchName);
                command.Parameters.AddWithValue("@batchSize", batch.BatchSize);
                command.Parameters.AddWithValue("@dateActive", batch.DateActive);
                command.Parameters.AddWithValue("@dateExpires", batch.DateExpires);
                batch.ID = Convert.ToInt32(command.ExecuteScalar());

                // Insert codes into the batch
                CreateDigitalCode(batch.BatchSize, batch.DateActive, command);

                // Commit transaction upon success
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
            Connection.Close();
        }

        public List<Batch> GetBatches()
        {
            // Create variable to store batches
            var batches = new List<Batch>();

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Batch";

                // Get all the batches
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
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

            Connection.Close();

            return batches;
        }

        public void CreateDigitalCode(int batchSize, DateTime dateActive, SqlCommand command)
        {
            // Loop through number of codes to generate
            using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open)))
            {
                // Get the next offset position
                var firstAndLastOffset = UpdateOffset(command, batchSize);
                if (firstAndLastOffset[0] % 4 != 0)
                {
                    throw new ArgumentException("Offset must be divisible by 4");
                }

                // Loop to the last offset position
                for (var i = firstAndLastOffset[0]; i < firstAndLastOffset[1]; i += 4)
                {
                    // Set reader to offset position
                    reader.BaseStream.Position = i;
                    var seedvalue = reader.ReadInt32();

                    // Insert code
                    command.Parameters.Clear();
                    command.CommandText = $@"INSERT INTO Codes (SeedValue, State) VALUES (@Seedvalue, @StateGenerated)";

                    // Insert values
                    command.Parameters.AddWithValue("@Seedvalue", seedvalue);
                    command.Parameters.AddWithValue("@StateGenerated", States.Generated);
                    command.ExecuteNonQuery();

                    // Update code to active state if dateActive is today
                    if (dateActive.Day == DateTime.Now.Day)
                    {
                        command.CommandText = "UPDATE Codes SET State = @StateActive WHERE SeedValue = @Seedvalue";
                        command.Parameters.AddWithValue("@StateActive", States.Active);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the next seed value that will be used to generate codes
        /// </summary>
        /// <returns></returns>
        public long[] UpdateOffset(SqlCommand command, int batchSize)
        {
            var firstAndLastOffset = new long[2];
            var offsetIncrement = batchSize * 4;

            command.CommandText = @"UPDATE Offset
                                   SET OffsetValue = OffsetValue + @offsetIncrement
                                   OUTPUT INSERTED.OffsetValue
                                   WHERE ID = 1";
            command.Parameters.AddWithValue("@offsetIncrement", offsetIncrement);
            var updatedOffset = (long)command.ExecuteScalar();

            // Set starting and ending offset positions
            firstAndLastOffset[0] = updatedOffset - offsetIncrement;
            firstAndLastOffset[1] = updatedOffset;

            return firstAndLastOffset;
        }

        public Code GetCode(string stringValue, string alphabet)
        {
            var seedValue = CodeConverter.ConvertFromCode(stringValue, alphabet);
            var code = new Code();

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Codes WHERE SeedValue = @seedValue";
                command.Parameters.AddWithValue("@seedValue", seedValue);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var seed = (int)reader["SeedValue"];
                        code.State = States.ConvertToString((byte)reader["State"]);
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);
                    }
                }
            }

            Connection.Close();

            return code;
        }

        /// <summary>
        /// Returns a list of all the codes from the database
        /// </summary>
        /// <returns></returns>
        public List<Code> GetCodes(int batchID, int pageNumber, string alphabet, int pageSize)
        {
            // Create list to store codes gathered from the database
            var codes = new List<Code>();

            Connection.Open();

            var p = Pagination.PaginationPageNumber(pageNumber, pageSize);

            using (var command = Connection.CreateCommand())
            {

                command.CommandText = @"DECLARE @codeIDStart int
                                        DECLARE @codeIDEnd int
                                        SET @codeIDStart = (SELECT CodeIDStart FROM Batch WHERE ID = @batchID)
                                        SET @codeIDEnd = (SELECT CodeIDEnd FROM Batch WHERE ID = @batchID)

                                        SELECT * FROM Codes WHERE ID BETWEEN @codeIDStart AND @codeIDEnd
                                        ORDER BY ID OFFSET @page ROWS FETCH NEXT @pageSize ROWS ONLY";

                command.Parameters.AddWithValue("@page", p);
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@batchID", batchID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        // Store code in a variable
                        var code = new Code();

                        //Stores SeedValue outside of code object
                        var seed = (int)reader["SeedValue"];

                        code.State = States.ConvertToString((byte)reader["State"]);
                        code.StringValue = CodeConverter.ConvertToCode(seed, alphabet);

                        // Add code to the list
                        codes.Add(code);
                    }
                }
            }

            Connection.Close();

            // Return the list of codes
            return codes;
        }

        public int PageCount(int id)
        {
            var pages = 0;

            var pagesRemainder = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT BatchSize FROM Batch WHERE ID = @id";

                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var numberOfCodes = (int)reader["BatchSize"];

                        pages = numberOfCodes / 10;

                        pagesRemainder = numberOfCodes % 10;

                        if (pagesRemainder > 0)
                        {
                            pages++;
                        }
                    }
                }
            }

            Connection.Close();

            return pages;
        }
        public void DeactivateCode(string code, string alphabet)
        {
            var seedvalue = CodeConverter.ConvertFromCode(code, alphabet);

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = @inactive
                                        WHERE SeedValue = @seedvalue AND [State] = @active";

                command.Parameters.AddWithValue("@inactive", States.Inactive);
                command.Parameters.AddWithValue("@active", States.Active);
                command.Parameters.AddWithValue("@seedvalue", seedvalue);
                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        public void DeactivateBatch(Batch batch)
        {
            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = @inactive
                                        WHERE ID BETWEEN @codeIDStart AND @codeIDEnd AND [State] = @active";

                command.Parameters.AddWithValue("@inactive", States.Inactive);
                command.Parameters.AddWithValue("@active", States.Active);
                command.Parameters.AddWithValue("@codeIDStart", batch.CodeIDStart);
                command.Parameters.AddWithValue("@codeIDEnd", batch.CodeIDEnd);
                command.ExecuteNonQuery();
            }

            Connection.Close();
        }

        public int CheckIfCodeCanBeRedeemed(string code, string alphabet)
        {
            var seedvalue = CodeConverter.ConvertFromCode(code, alphabet);
            int codeID = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = @redeemed
                                        OUTPUT INSERTED.ID
                                        WHERE SeedValue = @seedvalue AND [State] = @active";

                command.Parameters.AddWithValue("@redeemed", States.Redeemed);
                command.Parameters.AddWithValue("@active", States.Active);
                command.Parameters.AddWithValue("@seedvalue", seedvalue);

                codeID = (int)command.ExecuteScalar();
            }

            Connection.Close();

            if (codeID != 0)
            {
                return codeID;
            }
            return -1;
        }
    }
}