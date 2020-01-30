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

            // Create new codeJarClient
            var codeJarClient = new CodeJarClient(options);

            if(codeJarClient.CreateBatchAsync().Result != null)
            {
                Console.WriteLine("Batch created");
            }
            else
            {
                Console.WriteLine("Batch wasn't created");
            }

            // codeJarClient.GetBatchAsync(batch.ID).Wait();
        }
    }
}
