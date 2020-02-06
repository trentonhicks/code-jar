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

        //Testing for the correct state when a code is created.
        public async Task<bool> IsCodeStateCorrect(Batch batch)
        {
            var getCodes = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);
            var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[0];

            if (batch.DateActive <= DateTime.Now)
            {
                if (code.State == "Active")
                {
                    Console.WriteLine("Batch state is Active");
                    return true;
                }
            }
            else
            {
                if (code.State == "Generated")
                {
                    Console.WriteLine("Batch state is Generated");
                    return true;
                }
            }

            return false;
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
        public async Task<bool> PageComparison(Batch batch)
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
            return false;
        }

        //Testing for duplicate batches.
        public async Task<Batch> CreateBatch(Batch batch)
        {
            var newBatch = await _codeJarClient.CreateBatchAsync(batch);
            var deserialzedBatch = JsonSerializer.Deserialize<Batch>(await newBatch.Content.ReadAsStringAsync(), _jsonOptions);
            return deserialzedBatch;
        }

        public async Task<bool> TestingForDuplicateBatch(Batch batch)
        {

            var newBatch1 = await CreateBatch(batch);
            var newBatch2 = await CreateBatch(batch);
            var newBatch3 = await CreateBatch(batch);

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
            return true;
        }

        //Testing if the offset updates correctly
        public async Task<bool> TestingForOffset(Batch batch)
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

        public async Task<bool> DeactivateCode(Batch batch)
        {
            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[0];

            string failed = "Single code deactivation failed";

            if (code.State == "Active")
            {
                var deactivateCode = await _codeJarClient.DeactivateCodeAsync(code.StringValue);

                Console.WriteLine("A single code was deactivated");
                return true;
            }
            else if (code.State == "Generated")
            {
                Console.WriteLine(failed);
                return false;
            }
            else if (code.State == "Expired")
            {
                Console.WriteLine(failed);
                return false;
            }
            else if (code.State == "Redeemed")
            {
                Console.WriteLine(failed);
                return false;
            }
            else if (code.State == "Inactive")
            {
                Console.WriteLine("Already Inactive");
                return false;
            }
            return true;
        }

        public async Task<bool> DeactivateBatch(Batch batch)
        {
            string failed = "Batch deactivation failed";

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
                        return true;
                    }
                    else
                    {
                        stringValue++;
                    }
                }
                else if (code.State == "Generated")
                {
                    Console.WriteLine(failed);
                    return false;
                }
                else if (code.State == "Expired")
                {
                    Console.WriteLine(failed);
                    return false;
                }
                else if (code.State == "Redeemed")
                {
                    Console.WriteLine(failed);
                    return false;
                }
                else if (code.State == "Inactive")
                {
                    Console.WriteLine("Already Inactive");
                    return false;
                }
            }
            return true;
        }
    }
}