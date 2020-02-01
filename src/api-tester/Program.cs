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

            // Get list of batches
            var response = codeJarClient.GetBatchListAsync().Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var batches = JsonSerializer.Deserialize<List<Batch>>(content, options);
        }
    }
}