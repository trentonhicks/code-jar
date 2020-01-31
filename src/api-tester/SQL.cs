using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace api_tester
{
    public class SQL
    {
        public SqlConnection Connection { get; set; } = new SqlConnection("Data Source=.; Initial Catalog=RandomCode; Integrated Security=SSPI;");

        public async Task<Batch> CreateBatchWithMultipleStates()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var codeJarClient = new CodeJarClient(options);
            var batch = await codeJarClient.CreateBatchAsync();
            
            Connection.Open();

            using(var command = new SqlCommand())
            {
                command.CommandText = @"
                UPDATE Codes SET State = @redeemed WHERE ID = @codeIDStart
                UPDATE Codes SET State = @inactive WHERE ID = @codeIDEnd";

                command.Parameters.AddWithValue("@redeemed", States.Redeemed);
                command.Parameters.AddWithValue("@inactive", States.Inactive);
                command.Parameters.AddWithValue("@codeIDStart", batch.CodeIDStart);
                command.Parameters.AddWithValue("@codeIDEnd", batch.CodeIDEnd);

                command.ExecuteNonQuery();
            }

            Connection.Close();

            return batch;
        }
        public int GetNumberOfCodes(Batch batch)
        {
            int numberOfCodes;

            Connection.Open();
            {

                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = @"SELECT COUNT(Codes.ID)
                    From Batch
                    INNER JOIN
                    Codes
                    ON Codes.ID BETWEEN CodeIDStart AND CodeIDEnd
                    WHERE Batch.ID = @ID";

                    command.Parameters.AddWithValue("@ID", batch.ID);
                    numberOfCodes = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            Connection.Close();

            if (numberOfCodes == batch.BatchSize)
            {
                return numberOfCodes;
            }
            return -1;
        }


    }
}
