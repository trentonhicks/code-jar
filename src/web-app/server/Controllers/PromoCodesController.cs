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
        public IActionResult Get([FromQuery] string stringValue)
        {
            var connectionString = _config.GetConnectionString("Storage");
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var filepath = _config.GetSection("BinaryFile")["Binary"];

            var sql = new SQL(connectionString, filepath);
            var code = sql.GetCode(stringValue, alphabet);

            if(code.StringValue == stringValue)
            {
                return Ok(code);
            }
            else
            {
                return NotFound();
            }
        }

        //Set code status to inactive
        [HttpDelete]
        public void Delete([FromBody]string[] code)
        {
            var connectionString = _config.GetConnectionString("Storage");
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var filepath = _config.GetSection("BinaryFile")["Binary"];
            var sql = new SQL(connectionString, filepath);

            for (var i = 1; i <= code.Length; i++)
            {
                sql.DeactivateCode(code[i - 1], alphabet);
            }
        }
    }
}
