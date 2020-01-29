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

            var batch = codeJarClient.CreateBatchAsync(options).Result;
            codeJarClient.GetBatchAsync(batch.ID).Wait();
        }
    }
}
