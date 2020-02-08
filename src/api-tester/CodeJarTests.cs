using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace api_tester
{
    public class CodeJarTests
    {
        private CodeJarClient _codeJarClient = new CodeJarClient();
        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        public async Task<Batch> CreateActiveBatch(Batch batch)
        {
            var today = DateTime.Now;
            batch = new Batch()
            {
                BatchName = "Batch",
                BatchSize = 3,
                DateActive = today,
                DateExpires = today.AddDays(30)
            };

            var newBatch = await _codeJarClient.CreateBatchAsync(batch);
            var deserialzedBatch = JsonSerializer.Deserialize<Batch>(await newBatch.Content.ReadAsStringAsync(), _jsonOptions);
            return deserialzedBatch;
        }

        public async Task<Batch> CreateGeneratedBatch(Batch batch)
        {
            var today = DateTime.Now;
            batch = new Batch()
            {
                BatchName = "Generated Batch",
                BatchSize = 1,
                DateActive = today.AddDays(100),
                DateExpires = today.AddDays(150)
            };

            var newBatch = await _codeJarClient.CreateBatchAsync(batch);
            var deserialzedBatch = JsonSerializer.Deserialize<Batch>(await newBatch.Content.ReadAsStringAsync(), _jsonOptions);
            return deserialzedBatch;
        }

        //Calculation for pagination
        public int PageCalculator(Batch batch)
        {
            var pages = 0;
            var pageRemainder = 0;
            var numberOfCodes = batch.BatchSize;

            pages = numberOfCodes / 10;
            pageRemainder = numberOfCodes % 10;

            if (pageRemainder > 0)
            {
                pages++;
            }

            return pages;
        }

        //Comparing the local pages calculated and the pages calculated from the API.
        public async Task<bool> PaginationTest(Batch batch)
        {
            var client = new CodeJarClient();
            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);
            var tableData = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions);
            var apiPages = tableData.Pages;
            var localPages = PageCalculator(batch);
            if (localPages == apiPages)
            {
                return true;
            }
            Console.WriteLine("\n-PageComparison test FAILED!!!");
            return false;
        }

        //Testing for duplicate batches.
        public async Task<bool> DuplicateBatchesTest(Batch batch)
        {

            var newBatch1 = await CreateActiveBatch(batch);
            var newBatch2 = await CreateActiveBatch(batch);
            var newBatch3 = await CreateActiveBatch(batch);

            var batchList = await _codeJarClient.GetBatchListAsync();

            var desBatch = JsonSerializer.Deserialize<List<Batch>>(await batchList.Content.ReadAsStringAsync(), _jsonOptions);

            var c = 0;
            for (int i = 0; i < desBatch.Count; i++)
            {
                c = 0;
                for (int j = 0; j < desBatch.Count; j++)
                {
                    if (desBatch[i] == desBatch[j])
                    {
                        c++;
                    }
                    if (c > 2)
                    {
                        return false;
                    }
                }
            }
            Console.WriteLine("\n-No Duplicate Batches");
            return true;
        }

        //Testing if the offset updates correctly
        public async Task<bool> OffsetTest(Batch batch)
        {
            var batchList = await _codeJarClient.GetBatchListAsync();

            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var tableData = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions);

            var allCodes = tableData.Codes;

            var desBatch = JsonSerializer.Deserialize<List<Batch>>(await batchList.Content.ReadAsStringAsync(), _jsonOptions);

            var c = 0;
            for (int i = 0; i < desBatch.Count; i++)
            {
                for (int j = 0; j < allCodes.Count; j++)
                {
                    c = 0;
                    for (int k = 0; k < allCodes.Count; k++)
                    {
                        if (allCodes[j] == allCodes[k])
                        {
                            c++;
                        }
                        if (c > 2)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public async Task<bool> SearchForCodeTest(Batch batch)
        {
            //get batch
            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);
            //gets first code from batch 
            var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[0];
            //get the string value
            var stringValue = await _codeJarClient.SearchCodeAsync(code.StringValue);
            //deserialize the string to code
            var desStringValue = JsonSerializer.Deserialize<Code>(await stringValue.Content.ReadAsStringAsync(), _jsonOptions).StringValue;

            var compareCode = JsonSerializer.Deserialize<Code>(await stringValue.Content.ReadAsStringAsync(), _jsonOptions).StringValue;

            var test = string.Equals(desStringValue, compareCode);

            return test;
        }

        //Testing for the correct state when a code is created.
        public async Task<bool> CodeStateTest(Batch batch)
        {

            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);
            var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[0];

            if (batch.DateActive <= DateTime.Now)
            {
                if (code.State == "Active")
                {
                    Console.WriteLine("\n-Batch state is Active");
                    return true;
                }
            }
            else
            {
                if (code.State == "Generated")
                {
                    Console.WriteLine("\n-Batch state is Generated");
                    return true;
                }
            }

            return false;
        }
        public async Task<bool> DeactivateCodeTest(Batch batch)
        {
            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[0];

            string failedMsg = "DeactivateCode test FAILED!!!";

            if (code.State == "Active")
            {
                await _codeJarClient.DeactivateCodeAsync(code.StringValue);

                Console.WriteLine("\n-A single code was deactivated");
                return true;
            }
            else if (code.State == "Generated")
            {
                Console.WriteLine(failedMsg);
                return false;
            }
            else if (code.State == "Expired")
            {
                Console.WriteLine(failedMsg);
                return false;
            }
            else if (code.State == "Redeemed")
            {
                Console.WriteLine(failedMsg);
                return false;
            }
            else if (code.State == "Inactive")
            {
                Console.WriteLine($"\n-Code already Inactive.{failedMsg}");
                return false;
            }
            return true;
        }
        public async Task<bool> DeactivateBatchTest(Batch batch)
        {
            string failedMsg = "\nBatch deactivation test FAILED!!!";

            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var stringValue = new TableData().Codes.Count;

            for (int i = -1; i < stringValue; i++)
            {
                var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[stringValue];

                if (code.State == "Active")
                {
                    await _codeJarClient.DeactivateCodeAsync(code.StringValue);

                    if (stringValue == batch.BatchSize - 1)
                    {
                        System.Console.WriteLine("\n-Batch was Deactivated");
                        return true;
                    }
                    else
                    {
                        stringValue++;
                    }
                }
                else if (code.State == "Generated")
                {
                    Console.WriteLine(failedMsg);
                    return false;
                }
                else if (code.State == "Expired")
                {
                    Console.WriteLine(failedMsg);
                    return false;
                }
                else if (code.State == "Redeemed")
                {
                    Console.WriteLine(failedMsg);
                    return false;
                }
                else if (code.State == "Inactive")
                {
                    Console.WriteLine($"\n-Already Inactive. {failedMsg}");
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> RedeemCodeTest(Batch batch)
        {
            var failedMsg = "\nRedeem Code test FAILED!!!";

            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[0];

            if (code.State == "Active")
            {
                await _codeJarClient.RedeemCodeAsync(code.StringValue);

                Console.WriteLine("\n-A single code was Redeemed.");
                return true;
            }
            else if (code.State == "Redeemed")
            {
                Console.WriteLine($"\n-Code was already Redeemed. {failedMsg}");
                return false;
            }
            else if (code.State == "Generated")
            {
                Console.WriteLine(failedMsg);
                return false;
            }
            else if (code.State == "Expired")
            {
                Console.WriteLine(failedMsg);
                return false;
            }
            else if (code.State == "Inactive")
            {
                Console.WriteLine(failedMsg);
                return false;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> CodeStateChangeTest(Batch generatedBatch, Batch batch)
        {
            var genResponse = await _codeJarClient.GetBatchAsync(generatedBatch.ID, 1);
            var actResponse = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var genTableData = JsonSerializer.Deserialize<TableData>(await genResponse.Content.ReadAsStringAsync(), _jsonOptions);
            var actTableData = JsonSerializer.Deserialize<TableData>(await actResponse.Content.ReadAsStringAsync(), _jsonOptions);

            //code 1 is Generated. 
            var genBatchCode = genTableData.Codes[0];

            //Test if a Generated code can be  Redeemed/Deactivated
            var redeem = await _codeJarClient.RedeemCodeAsync(genBatchCode.StringValue);
            if (!(redeem.IsSuccessStatusCode))
            {
                Console.WriteLine("\n-Test Successful. Failed to Redeem a Generated code.");
            }

            var deactivate = await _codeJarClient.DeactivateCodeAsync(genBatchCode.StringValue);
            if (deactivate.IsSuccessStatusCode)
            {
                Console.WriteLine("-Test Successful. Failed to Deactivate a Generated code.");
            }

            //Set to Redeemed state. Active>Redeemed
            var activeCodeOne = actTableData.Codes[0];
            await _codeJarClient.RedeemCodeAsync(activeCodeOne.StringValue);

            //Try to redeem redeemed code
            var redeem2 = await _codeJarClient.RedeemCodeAsync(activeCodeOne.StringValue);
            if (!(redeem2.IsSuccessStatusCode))
            {
                Console.WriteLine("-Test Successful. Failed to Redeem a Redeemed code.");
            }

            var deactivate2 = await _codeJarClient.DeactivateCodeAsync(activeCodeOne.StringValue);
            if (deactivate2.IsSuccessStatusCode)
            {
                Console.WriteLine("-Test Successful. Failed to Deactivate a Redeemed code.");
            }

            //Set code to Inactive
            var activeCodeTwo = actTableData.Codes[1];
            await _codeJarClient.DeactivateCodeAsync(activeCodeTwo.StringValue);

            //Try to Redeem Inactive code
            var redeem3 = await _codeJarClient.RedeemCodeAsync(activeCodeTwo.StringValue);
            if (!(redeem2.IsSuccessStatusCode))
            {
                Console.WriteLine("-Test Successful. Failed to Redeem an Inactive code.");
            }
            //Try to Deactivate Inactive code
            var deactivate3 = await _codeJarClient.DeactivateCodeAsync(activeCodeTwo.StringValue);
            if (deactivate3.IsSuccessStatusCode)
            {
                Console.WriteLine("-Test Successful. Failed to Deactivate an Inactive code.");
            }
            return true;
        }
    }
}