using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public async Task GetBatchAsync(int batchID)
        {
            var response = await Client.GetStringAsync($"http://localhost:5000/batch/{batchID}?page=1");
            Console.WriteLine(response);
        }
        public async Task<Batch> CreateBatchAsync()
        {
            var payload = "{\"BatchName\": \"foo\",\"BatchSize\": 20, \"DateActive\": \"2020-01-29\", \"DateExpires\": \"2020-01-30\"}";
            HttpContent postBody =  new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("http://localhost:5000/batch", postBody);
            var responseContent = await response.Content.ReadAsStringAsync();
            var batch = JsonSerializer.Deserialize<Batch>(responseContent, JsonOptions);

            if (response.IsSuccessStatusCode)
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
    }
}