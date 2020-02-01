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
            // Create new codeJarClient
            var codeJarClient = new CodeJarClient();

            // Create new batch instance
            var batch = new Batch
            {
                BatchName = "Batch",
                BatchSize = 10,
                DateActive = new DateTime(year: 2020, month: 2, day: 1),
                DateExpires = new DateTime(year: 2020, month: 2, day: 15)
            };

            // Create batch with API
            var response = codeJarClient.CreateBatchAsync(batch).Result;
            var content = response.Content.ReadAsStringAsync();

            Console.ReadLine();
        }
    }
}