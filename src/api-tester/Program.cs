using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace api_tester
{
    class Program
    {
        static void Main(string[] args)
        {
           var id = CreateBatchAsync().Result;
           GetBatchAsync(id).Wait();

        }

        static async Task GetBatchAsync(int batchID)
        {
            var http = new HttpClient();
            var response = await http.GetStringAsync($"http://localhost:5000/batch/{batchID}?page=1");
            Console.WriteLine(response);
        }

        static async Task<int> CreateBatchAsync()
        {
            var http = new HttpClient();
            var payload = "{\"BatchName\": \"foo\",\"BatchSize\": 20, \"DateActive\": \"2020-01-28\", \"DateExpires\": \"2020-01-29\"}";
            HttpContent foo =  new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await http.PostAsync("http://localhost:5000/batch", foo);

            var content = Convert.ToInt32(await response.Content.ReadAsStringAsync());
            Console.WriteLine(content);
            return content;

        }
    }
}
