using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Diagnostics;

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
            var test = new CodeJarTests();

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

            //seperate batch for RedeemCode
            var batchResponse2 = codeJarClient.CreateBatchAsync(batch).Result;
            var batchContent2 = batchResponse2.Content.ReadAsStringAsync().Result;
            var secondBatch = JsonSerializer.Deserialize<Batch>(batchContent2, options);

            // seperate batch for DeactivateBatch
            var batchResponse3 = codeJarClient.CreateBatchAsync(batch).Result;
            var batchContent3 = batchResponse3.Content.ReadAsStringAsync().Result;
            var thirdBatch = JsonSerializer.Deserialize<Batch>(batchContent3, options);

            //Checking if pagination works
            if (test.PaginationTest(createdBatch).Result)
            {
                Console.WriteLine("\nPagination test SUCCESSFUL.");
            }

            //Testing if there are dupicate batches
            if (test.DuplicateBatchesTest(createdBatch).Result)
            {
                Console.WriteLine("DuplicateBatches test SUCCESSFUL");
            }

            //Testing if the offset updates correctly.
            if (test.OffsetTest(createdBatch).Result)
            {
                Console.WriteLine("\nOffset test SUCCESSFUL");
            }

            //testing if you can search
            if (test.SearchForCodeTest(createdBatch).Result)
            {
                Console.WriteLine("\nSearchForCode test SUCCESSFUL");
            }
            
            //Checking if Codes Generated State is correct.
            if (test.CodeStateTest(createdBatch).Result)
            {
                Console.WriteLine("CodeState test SUCCESSFUL");
            }

            //testing if you can deactivate a code
            if (test.DeactivateCodeTest(createdBatch).Result)
            {
                Console.WriteLine("DeactivateCode test SUCCESSFUL");
            }

            //testing if you can deactivate a batch
            if (test.DeactivateBatchTest(thirdBatch).Result)
            {
                Console.WriteLine("DeactiveBatch test SUCCESSFUL");
            }

            //testing to redeem a single code
            if (test.RedeemCodeTest(secondBatch).Result)
            {
                Console.WriteLine("RedeemCode test SUCCESSFUL");
            }

            //test to redeem a code that has been redeemed
            if (test.RedeemRedeemedCodeTest(secondBatch).Result)
            {
                Console.WriteLine("Redeem-Redeemed-Code test SUCCESSFUL");
            }

            //test if you can delete a batch
            if(test.DeleteBatchTest(createdBatch).Result)
            {
                Console.WriteLine("Can delete a batch");
            }
        }
    }
}