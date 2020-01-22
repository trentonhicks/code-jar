using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public BatchController(ILogger<PromoCodesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet("batch")]
        public List<Batch> Get()
        {
            var sql = new SQL(_config.GetConnectionString("Storage"));
            return sql.GetBatches();
        }

        [HttpGet("batch/{id}")]
        public TableData GetCodes(int id, [FromQuery] int page, [FromQuery] string stringValue, [FromQuery] string state)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var sql = new SQL(_config.GetConnectionString("Storage"));
            var codes = sql.GetCodes(id, page, stringValue, state, alphabet);
            var pages = sql.PageCount(id);

            return new TableData(codes, pages);
        }

        [HttpDelete("batch")]
        public void DeactivateBatch([FromBody] Batch batch)
        {
            var sql = new SQL(_config.GetConnectionString("Storage"));
            sql.DeactivateBatch(batch);
        }

        [HttpPost("batch")]
        public IActionResult Post(Batch batch, DateTime dateActive, DateTime dateExpires)
        {
             // Date active must be less than date expires and greater than or equal to the current date time in order to generate codes

             if(dateActive < dateExpires && dateActive.Day >= DateTime.Now.Day)
             {
                            // Create CodeGenerator instance
                        var codeGenerator = new CodeGenerator(
                            _config.GetConnectionString("Storage"),
                            _config.GetSection("BinaryFile")["Binary"]
                        );

                        // Get the CodeIDStart and CodeIDEnd values for the batch and store them as part of the batch
                        var sql = new SQL(_config.GetConnectionString("Storage"));
                        var codeIDStartAndEnd = sql.GetCodeIDStartAndEnd(batch.BatchSize);

                        // Start value
                        batch.CodeIDStart = codeIDStartAndEnd[0];

                        // End value
                        batch.CodeIDEnd = codeIDStartAndEnd[1];

                        // Create batch
                        sql.CreateBatch(batch, codeGenerator);

                        return Ok();
             }
             else
             {
                 return BadRequest();
             }
         
        }
    }
}
