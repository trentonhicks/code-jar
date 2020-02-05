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

            var batch = new Batch()
            {
                BatchName = "Batch",
                BatchSize = 10,
                DateActive = new DateTime(year: 2020, month: 2, day: 1),
                DateExpires = new DateTime(year: 2020, month: 2, day: 15)
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

            //Checking if pagination works
            if(test.PageComparison(createdBatch).Result)
            {
                Console.WriteLine("Pagination works");
            }
        }
    }
}