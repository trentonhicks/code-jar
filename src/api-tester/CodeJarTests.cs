using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace api_tester
{
    public class CodeJarTests
    {

        private static CodeJarClient _codeJarClient = new CodeJarClient(_jsonOptions);
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        };

        public static async Task<bool> IsCodeStateCorrect (Batch batch, int batchID)
        {

            var getCodes = await _codeJarClient.GetBatchAsync(batchID);

            var code = getCodes.Codes[0];

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