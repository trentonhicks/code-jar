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
        public TableData Get([FromQuery]int page)
        {
            var connectionString = _config.GetConnectionString("Storage");

            var sql = new SQL(connectionString);

            // Get 10 codes from the database based on the page you are on in the app
            var codes = sql.GetCodes(page);
            var pages = sql.PageCount();

            var tableData = new TableData(codes, pages);
            
            return tableData;
        }

        [HttpPost]
        public void Post([FromBody] int numberOfCodes)
        {
            var connectionString = _config.GetConnectionString("Storage");

            var filePath = _config.GetSection("BinaryFile")["Binary"];

            var cGenerate = new CodeGenerator(connectionString, filePath);

            //Creates n number of codes and stores them in DB
            cGenerate.CreateDigitalCode(numberOfCodes);
        }

        //Set code status to inactive
        [HttpDelete]
        public void Delete([FromBody]string [] code)
        {

            var connectionString = _config.GetConnectionString("Storage");
            var alphabet = _config.GetSection("Base26")["alphabet"];

            var sql = new SQL(connectionString);

            for(var i = 1; i <= code.Length; i++)
            {
                sql.InactiveStatus(code[i - 1], alphabet);
            }
            
        }
    }
}
