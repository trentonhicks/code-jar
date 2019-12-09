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
    [Route("codes")]
    public class PromoCodesController : ControllerBase
    {
        private readonly ILogger<PromoCodesController> _logger;
        private readonly IConfiguration _config;

        public PromoCodesController(ILogger<PromoCodesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public List<Code> Get()
        {
            var connectionString = _config.GetConnectionString("Storage");

            var sql = new SQL(connectionString);

            // Get the list of codes from the database
            var codes = sql.GetCodes();

            return codes;
        }

        [HttpPost]
        public void Post([FromBody] int numberOfCodes)
        {
            var connectionString = _config.GetConnectionString("Storage");

            var filePath = _config.GetConnectionString("Binary");


            var cGenerate = new CodeGenerator(connectionString, filePath);

            //Creates n number of codes and stores them in DB
            cGenerate.CreateDigitalCode(numberOfCodes);
        }

        //Set code status to inactive
        [HttpDelete]
        public void Delete([FromBody]int[] codeID)
        {

        }
    }
}
