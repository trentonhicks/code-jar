using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data.SqlClient;
using System.Collections.Generic;



namespace api_tester
{
    public class CodeJarClient
    {
        public CodeJarClient(JsonSerializerOptions options)
        {
            JsonOptions = options;
        }


        public JsonSerializerOptions JsonOptions {get; set;}


        public HttpClient Client = new HttpClient();


        public async Task<TableData> GetBatchAsync(int batchID)
        {
            var response = await Client.GetStringAsync($"http://localhost:5000/batch/{batchID}?page=1");
            var td = JsonSerializer.Deserialize<TableData>(response, JsonOptions);
            return td;
        }



        
        /// <summary>
        /// Creates a batch using the /batch endpoint.
        /// </summary>
        /// <returns>
        /// Returns the batch that was created or null if the API failed.
        /// </returns>



        public async Task<Batch> CreateBatchAsync()
        {
            var payload = "{\"BatchName\": \"foo\",\"BatchSize\": 20, \"DateActive\": \"2020-01-30\", \"DateExpires\": \"2020-01-31\"}";
            HttpContent postBody = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("http://localhost:5000/batch", postBody);
            var responseContent = await response.Content.ReadAsStringAsync();
            var batch = JsonSerializer.Deserialize<Batch>(responseContent, JsonOptions);

            if (response.IsSuccessStatusCode && batch.DateActive.Date >= DateTime.Now.Date && batch.DateExpires > batch.DateActive)
            {
                // Compare the post body with the batch stored in the database
                var postedBatch = JsonSerializer.Deserialize<Batch>(payload);

                if (batch.BatchName == postedBatch.BatchName
                    && batch.BatchSize == postedBatch.BatchSize
                    && batch.DateActive == postedBatch.DateActive
                    && batch.DateExpires == postedBatch.DateExpires)
                    {
                        return batch;
                    }
            }

            return null;
        }




        /// <summary>
        /// Checks if codes can be redeemed and if the API follows the state machine.
        /// </summary>
        /*public async Task<bool> RedeemCodeAsync(Batch batch)
        {
            var activeCode;
            var generatedCode;
            var inactiveCode;
            var redeemedCode;
            
            var postBody = new StringContent("", Encoding.UTF8, "application/json");
            Client.PostAsync("http://localhost:5000/redeem-code", );
        }*/


        

         


    }
}