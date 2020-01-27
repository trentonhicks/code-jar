using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;


namespace CodeJar.WebApp
{
    public class SQL
    {
        public SQL(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        // SQL connection string
        public SqlConnection Connection { get; set; }

        /// <summary>
        /// Stores codes in the database
        /// </summary>
        /// <param name="code"></param>
        /// <param name="offset"></param>
       

        public void CreateBatch(Batch batch, CodeGenerator codeGenerator)
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
                VALUES(@batchName, @codeIDStart, @batchSize, @dateActive, @dateExpires)";

                command.Parameters.AddWithValue("@batchName", batch.BatchName);
                command.Parameters.AddWithValue("@batchSize", batch.BatchSize);
                command.Parameters.AddWithValue("@dateActive", batch.DateActive);
                command.Parameters.AddWithValue("@dateExpires", batch.DateExpires);
                command.ExecuteNonQuery();

                // Insert codes into the batch
                codeGenerator.CreateDigitalCode(batch.BatchSize, batch.DateActive, batch.DateExpires, command);

                // Commit transaction upon success
                transaction.Commit();
            }
            catch(Exception ex)
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
            
            using(var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Batch";

                // Get all the batches
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
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

        /// <summary>
        /// Gets the next seed value that will be used to generate codes
        /// </summary>
        /// <returns></returns>
        public long[] GetOffset(SqlCommand command, int batchSize)
        {
            var firstAndLastOffset = new long[2];
            var offsetIncrement = batchSize * 4;

            command.CommandText = @"UPDATE Offset
                                   SET OffsetValue = OffsetValue + @offsetIncrement
                                   OUTPUT INSERTED.OffsetValue
                                   WHERE ID = 1";
            command.Parameters.AddWithValue("@offsetIncrement", offsetIncrement);

            using (var reader = command.ExecuteReader())
            {
                if(reader.Read())
                {
                    firstAndLastOffset[0] = (long)reader["OffsetValue"] - offsetIncrement;
                    firstAndLastOffset[1] = (long)reader["OffsetValue"];
                }
            }
            return firstAndLastOffset;
        }

        public Code GetCode(string stringValue, string alphabet)
        {
            var seedValue = CodeConverter.ConvertFromCode(stringValue, alphabet);
            var code = new Code();

            Connection.Open();

            using(var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Codes WHERE SeedValue = @seedValue";
                command.Parameters.AddWithValue("@seedValue", seedValue);

                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read()) {
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

            using(var command = Connection.CreateCommand())
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

                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
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


                  using(var reader = command.ExecuteReader())
                  {
                     

                      while(reader.Read())
                      {
                        
                        var numberOfCodes = (int)reader["BatchSize"] ;

                        pages = numberOfCodes / 10;

                        pagesRemainder = numberOfCodes % 10;

                        if(pagesRemainder > 0)
                        {
                            pages++;
                        }
                      }
                  }
             }

             Connection.Close();

             return pages;
         }
        public void InactiveStatus(string code, string alphabet)
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

            using(var command = Connection.CreateCommand())
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

        public bool CheckIfCodeCanBeRedeemed(string code, string alphabet)
        {
            var seedvalue = CodeConverter.ConvertFromCode(code, alphabet);
            int recordsAffected = 0;

            Connection.Open();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Codes SET [State] = @redeemed
                                        WHERE SeedValue = @seedvalue AND [State] = @active";

                command.Parameters.AddWithValue("@redeemed", States.Redeemed);
                command.Parameters.AddWithValue("@active", States.Active);
                command.Parameters.AddWithValue("@seedvalue", seedvalue);

                recordsAffected = command.ExecuteNonQuery();
            }

            Connection.Close();

            if(recordsAffected > 0)
            {
                return true;
            }

            return false;
        }

        public int[] GetCodeIDStartAndEnd(int batchSize)
        {
            var codeIDStart = 1;

            // Get the last code generated and add one (this is the next code that will be generated)
            Connection.Open();

            using(var command = Connection.CreateCommand())
            {
                command.CommandText = @"SELECT TOP 1 ID FROM Codes ORDER BY ID DESC";
                
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        codeIDStart = (int)reader["ID"] + 1;
                    }
                }
            }

            Connection.Close();

            return new int[2] {codeIDStart, codeIDStart + batchSize - 1};
        }
    }
}