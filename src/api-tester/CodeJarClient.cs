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
        public HttpClient Client = new HttpClient();

        public async Task<HttpResponseMessage> GetBatchListAsync()
        {
            return await Client.GetAsync("http://localhost:5000/batch/");
        }

        /// <summary>
        /// Gets the first page of a batch
        /// </summary>
        public async Task<HttpResponseMessage> GetBatchAsync(int id, int page)
        {
            return await Client.GetAsync($"http://localhost:5000/batch/{id}?page={page}");
        }
        
        /// <summary>
        /// Creates a batch using the /batch route.
        /// </summary>
        public async Task<HttpResponseMessage> CreateBatchAsync(Batch batch)
        {
            // Format dates
            var dateActive = FormatDate.YearMonthDay(batch.DateActive);
            var dateExpires = FormatDate.YearMonthDay(batch.DateExpires);

            // Create content object
            HttpContent content = new StringContent(
                content: "{" +
                    $"\"BatchName\": \"{batch.BatchName}\"," +
                    $"\"BatchSize\": {batch.BatchSize}," +
                    $"\"DateActive\": \"{dateActive}\"," + 
                    $"\"DateExpires\": \"{dateExpires}\"" +
                "}",
                encoding: Encoding.UTF8,
                mediaType: "application/json"
           );
            
            // Return response
            return await Client.PostAsync("http://localhost:5000/batch", content);
        }

    }
}