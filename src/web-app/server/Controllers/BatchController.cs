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
using System.Data.SqlClient;
using CodeJar.WebApp.Commands;

namespace CodeJar.WebApp.Controllers
{
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IQueueClient _queueClient;
        private readonly ILogger<PromoCodesController> _logger;
        private readonly IConfiguration _config;
        private readonly IBatchRepository _batchRepository;
        private readonly ICodeRepository _codeRepository;
        private readonly SqlConnection _connection;

        public BatchController(
            ILogger<PromoCodesController> logger,
            IConfiguration config,
            IBatchRepository batchRepository,
            ICodeRepository codeRepository,
            SqlConnection connection,
            IQueueClient queueClient)
        {
            _logger = logger;
            _config = config;
            _batchRepository = batchRepository;
            _codeRepository = codeRepository;
            _connection = connection;
            _queueClient = queueClient;
        }

        [HttpGet("batch")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _batchRepository.GetBatchesAsync());
        }

        [HttpGet("batch/{id}")]
        public async Task<IActionResult> GetBatch(int id, [FromQuery] int page)
        {
            var pagination = new PaginationCount(_connection);
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var pageSize = Convert.ToInt32(_config.GetSection("Pagination")["PageNumber"]);
            var codes = await _codeRepository.GetCodesAsync(id, page, alphabet, pageSize);

            var vm = codes.Select( c => new CodeViewModel { Id = c.Id, State = c.State.ToString(), StringValue = c.StringValue });

            var pages = await pagination.PageCount(id);

            return Ok(new CodesViewModel(vm.ToList(), pages));
        }

        [HttpDelete("batch")]
        public async Task<IActionResult> DeactivateBatch([FromBody] Batch batch)
        {
            await _batchRepository.DeactivateBatchAsync(batch);
            return Ok();
        }

        [HttpPost("batch")]
        public async Task<IActionResult> Post(CreateBatchCommand request)
        {
            // Date active must be less than date expires and greater than or equal to the current date time in order to generate codes
            if (request.DateActive < request.DateExpires && request.DateActive.Date >= DateTime.Now.Date)
            {
                string messageBody = JsonConvert.SerializeObject(request);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                await _queueClient.SendAsync(message);
                
                return Ok(request);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
