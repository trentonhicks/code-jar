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
        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        };

        //Testing for the correct state when a code is created.

        public async Task<bool> IsCodeStateCorrect (Batch batch)
        {
            var getCodes = await _codeJarClient.GetBatchAsync(batch.ID, 1);

            var response = await _codeJarClient.GetBatchAsync(batch.ID, 1);
            var code = JsonSerializer.Deserialize<TableData>(await response.Content.ReadAsStringAsync(), _jsonOptions).Codes[0];

            if(batch.DateActive <= DateTime.Now)
            {
                if(code.State == "Active")
                {
                    return true;
                }
            }
            else
            {
                if(code.State == "Generated")
                {
                    return true;
                }
            }

            return false;
        }
    }
}