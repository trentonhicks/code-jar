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

            var batchCreated = codeJarClient.CreateBatchAsync().Result;


            if(batchCreated != null)
            {
                Console.WriteLine($"Batch ID {batchCreated.ID} created");

                var sql = new SQL();
                var result = sql.GetNumberOfCodes(batchCreated);

                if(result > 0)
                {
                    // run pagination method
                Console.WriteLine($"Batch has {batchCreated.BatchSize} codes");
                }
                else
                {
                    Console.WriteLine("No codes");                    
                }
            }
            else
            {
                Console.WriteLine("Batch wasn't created");
            }

            // codeJarClient.GetBatchAsync(batch.ID).Wait();
        }
    }
}
