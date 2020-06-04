using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeJar.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Newtonsoft.Json;
using CodeJar.Infrastructure;
using CodeJar.WebApp.ViewModels;

namespace CodeJar.WebApp.Controllers
{
    [ApiController]
    public class BatchController : ControllerBase
    {
        const string QueueName = "codejar";
        private QueueClient _queueClient;
        private readonly ILogger<PromoCodesController> _logger;
        private readonly IConfiguration _config;
        private readonly IBatchRepository _batchRepository;
        private readonly ICodeRepository _codeRepository;

        public BatchController(
            ILogger<PromoCodesController> logger,
            IConfiguration config,
            IBatchRepository batchRepository,
            ICodeRepository codeRepository)
        {
            _logger = logger;
            _config = config;
            _batchRepository = batchRepository;
            _codeRepository = codeRepository;
        }

        [HttpGet("batch")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _batchRepository.GetBatchesAsync());
        }

        [HttpGet("batch/{id}")]
        public async Task<IActionResult> GetBatch(int id, [FromQuery] int page)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var sql = new SQL(_config.GetConnectionString("Storage"), _config.GetSection("BinaryFile")["Binary"]);
            var pageSize = Convert.ToInt32(_config.GetSection("Pagination")["PageNumber"]);
            var codes = await _codeRepository.GetCodesAsync(id, page, alphabet, pageSize);

            var vm = codes.Select( c => new CodeViewModel { Id = c.Id, State = CodeStates.ConvertToString(c.State), StringValue = c.StringValue });

            var pages = sql.PageCount(id);

            return Ok(new CodesViewModel(vm.ToList(), pages));
        }

        [HttpDelete("batch")]
        public void DeactivateBatch([FromBody] Batch batch)
        {
            var sql = new SQL(_config.GetConnectionString("Storage"), _config.GetSection("BinaryFile")["Binary"]);
            sql.DeactivateBatch(batch);
        }

        [HttpPost("batch")]
        public async Task<IActionResult> Post(Batch batch)
        {
            // Date active must be less than date expires and greater than or equal to the current date time in order to generate codes
            if (batch.DateActive < batch.DateExpires && batch.DateActive.Date >= DateTime.Now.Date)
            {
                batch.State = BatchStates.Pending;

                await _batchRepository.AddBatchAsync(batch);

                var newBatch = await _batchRepository.GetBatchAsync(batch.ID);

                await _batchRepository.UpdateBatchAsync(batch);

                var connectionString = "Endpoint=sb://codefliptodo.servicebus.windows.net/;SharedAccessKeyName=web-app;SharedAccessKey=x9SEbxQ1AlykQv+ygjDh7hlVup1ZAOZkRTrhkuDHgJA=";

                _queueClient = new QueueClient(connectionString, QueueName);
                
                string messageBody = JsonConvert.SerializeObject(newBatch);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                await _queueClient.SendAsync(message);

                batch.State = BatchStates.Generated;

                await _batchRepository.UpdateBatchAsync(batch);

                await _queueClient.CloseAsync();

                return Ok(batch);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
