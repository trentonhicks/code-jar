using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace api_tester
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Create new codeJarClient
            var codeJarClient = new CodeJarClient();
            var today = DateTime.Now;
            var batch = new Batch()
            {
                BatchName = "Batch",
                BatchSize = 10,
                DateActive = today,
                DateExpires = today.AddDays(30)
            };
            var batchResponse = codeJarClient.CreateBatchAsync(batch).Result;
            var batchContent = batchResponse.Content.ReadAsStringAsync().Result;
            var createdBatch = JsonSerializer.Deserialize<Batch>(batchContent, options);

            var test = new CodeJarTests();

             // Checking if Codes Generated State is correct.
            if (test.IsCodeStateCorrect(createdBatch).Result)
            {
                Console.WriteLine("State when generated is correct.");
            }
        }
    }
}