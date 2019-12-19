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
    [Route("batchcodes")]
    public class BatchController : ControllerBase
    {
        private readonly ILogger<PromoCodesController> _logger;
        private readonly IConfiguration _config;

        public BatchController(ILogger<PromoCodesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public List<Batch> Get()
        {
            var sql = new SQL(_config.GetConnectionString("Storage"));
            return sql.GetBatches();
        }

        [HttpPost]
        public void Post(Batch batch)
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

            // Generate codes based on the number stored in batch size
            codeGenerator.CreateDigitalCode(batch.BatchSize);

            // Create batch
            sql.CreateBatch(batch);
        }
    }
}
