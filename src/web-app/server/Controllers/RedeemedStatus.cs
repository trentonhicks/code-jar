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
    [Route("redeem-code")]
    public class RedeemdedStatus : ControllerBase
    {
        private readonly ILogger<RedeemdedStatus> _logger;
        private readonly IConfiguration _config;

        public RedeemdedStatus(ILogger<RedeemdedStatus> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost]
        public IActionResult Post([FromBody] string code)
        {
            var connectionString = _config.GetConnectionString("Storage");

            var alphabet = _config.GetSection("Base26")["alphabet"];

            var sql = new SQL(connectionString);
            
            var response = sql.RedeemedStatus(code, alphabet);

            if(response)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
