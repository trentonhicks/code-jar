using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;

namespace api_tester
{
    public class SQL
    {
        public SqlConnection Connection { get; set; } = new SqlConnection("Data Source=.; Initial Catalog=RandomCode; Integrated Security=SSPI;");

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
