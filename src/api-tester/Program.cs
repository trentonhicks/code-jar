using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace api_tester
{
    class Program
    {
        static void Main(string[] args)
        {
            // JsonSerializer Options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var batch = CreateBatchAsync(options).Result;
            GetBatchAsync(batch.ID).Wait();
        }

        static async Task GetBatchAsync(int batchID)
        {
            var http = new HttpClient();
            var response = await http.GetStringAsync($"http://localhost:5000/batch/{batchID}?page=1");
            Console.WriteLine(response);
        }

        static async Task<Batch> CreateBatchAsync(JsonSerializerOptions options)
        {
            var http = new HttpClient();
            var payload = "{\"BatchName\": \"foo\",\"BatchSize\": 20, \"DateActive\": \"2020-01-29\", \"DateExpires\": \"2020-01-30\"}";
            HttpContent foo =  new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await http.PostAsync("http://localhost:5000/batch", foo);

            var content = await response.Content.ReadAsStringAsync();
            var batch = JsonSerializer.Deserialize<Batch>(content, options);

            Console.WriteLine(content);
            return batch;

        }
    }
}
