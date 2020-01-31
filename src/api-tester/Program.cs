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

            // Create batch with API
            var batchCreated = codeJarClient.CreateBatchAsync().Result;

            // Check if batch was created
            if(batchCreated != null)
            {
                Console.WriteLine($"Batch {batchCreated.ID} was created");

                var sql = new SQL();
                var numberOfCodes = sql.GetNumberOfCodes(batchCreated);

                // Check if number of codes in the batch is correct
                if(numberOfCodes > 0)
                {
                    Console.WriteLine($"Batch has {batchCreated.BatchSize} codes.");

                    // Check if the API returns the correct number of pages for a batch
                    
                    var pageNumberIsCorrect = PageComparison(batchCreated).Result;

                    if(pageNumberIsCorrect)
                    {
                        Console.WriteLine("Page number is correct");
                    }
                }
                else
                {
                    Console.WriteLine("The batch has an innacurate number of codes.");                    
                }
            }
            else
            {
                Console.WriteLine("Batch wasn't created");
            }

            // codeJarClient.GetBatchAsync(batch.ID).Wait();
        }



        public static int PageCalculator(Batch batch)
        {
            var pages = 0;
            var pageRemainder = 0;
            var numberOfCodes = batch.BatchSize;

            pages = numberOfCodes / 10;
            pageRemainder = numberOfCodes % 10;

            if(pageRemainder > 0)
            {
                pages++;
            }

            return pages;

        }

        public static async Task<bool> PageComparison(Batch batch)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var client = new CodeJarClient(options);

            var tableData = await client.GetBatchAsync(batch.ID);

            var apiPages = tableData.Pages;

            var localPages = PageCalculator(batch);

            if(localPages == apiPages)
            {
                return true;
            }
            return false;


        }
        
    }
}