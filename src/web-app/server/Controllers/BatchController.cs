using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeJar.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeJar.WebApp.Controllers
{
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly ILogger<PromoCodesController> _logger;
        private readonly IConfiguration _config;
        private readonly IBatchRepository _batchRepository;

        public BatchController(
            ILogger<PromoCodesController> logger,
            IConfiguration config,
            IBatchRepository batchRepository)
        {
            _logger = logger;
            _config = config;
            _batchRepository = batchRepository;
        }

        [HttpGet("batch")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _batchRepository.GetBatchesAsync());
        }

        [HttpGet("batch/{id}")]
        public IActionResult GetBatch(int id, [FromQuery] int page)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var sql = new SQL(_config.GetConnectionString("Storage"), _config.GetSection("BinaryFile")["Binary"]);
            var pageSize = Convert.ToInt32(_config.GetSection("Pagination")["PageNumber"]);
            var codes = sql.GetCodes(id, page, alphabet, pageSize);
            var pages = sql.PageCount(id);

            return Ok(new TableData(codes, pages));
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
                // Create batch
                await _batchRepository.CreateBatchAsync(batch);

                return Ok(batch);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
