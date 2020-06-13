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
        private readonly PaginationCount _pagination;
        private readonly ILogger<CodesController> _logger;
        private readonly IConfiguration _config;
        private readonly IBatchRepository _batchRepository;
        private readonly ICodeRepository _codeRepository;

        public BatchController(
            ILogger<CodesController> logger,
            IConfiguration config,
            IBatchRepository batchRepository,
            ICodeRepository codeRepository,
            IQueueClient queueClient,
            PaginationCount paginationCount)
        {
            _logger = logger;
            _config = config;
            _batchRepository = batchRepository;
            _codeRepository = codeRepository;
            _queueClient = queueClient;
            _pagination = paginationCount;
        }

        [HttpGet("batch")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _batchRepository.GetBatchesAsync());
        }

        [HttpGet("batch/{id}")]
        public async Task<IActionResult> GetBatch(Guid id, [FromQuery] int page)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var pageSize = Convert.ToInt32(_config.GetSection("Pagination")["PageNumber"]);
            var codes = await _codeRepository.GetCodesAsync(id, page, pageSize);

            var vm = codes.Select( c => new CodeViewModel { Id = c.Id, State = c.State.ToString(), StringValue = CodeConverter.ConvertToCode(c.SeedValue, alphabet) });

            var pages = await _pagination.PageCount(id);

            return Ok(new CodesViewModel(vm.ToList(), pages));
        }

        [HttpDelete("batch")]
        public async Task<IActionResult> DeactivateBatch([FromBody] Batch batch)
        {
            await _batchRepository.DeactivateBatchAsync(batch.Id);
            return Ok();
        }

        [HttpPost("batch")]
        public async Task<IActionResult> Post(CreateBatchCommand request)
        {
            string messageBody = JsonConvert.SerializeObject(request);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await _queueClient.SendAsync(message);
            
            return Ok(request);
        }
    }
}
