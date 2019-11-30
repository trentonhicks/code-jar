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
    [Route("generatedcodes")]
    public class GenerateCodes : ControllerBase
    {
        private readonly ILogger<GenerateCodes> _logger;
        private readonly IConfiguration _config;

        public GenerateCodes(ILogger<GenerateCodes> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public void Get(int amount)
        {
            var connectionString = _config.GetConnectionString("Storage");

            var generateCodes = new CodeGenerator(connectionString);

            // Creates Codes and stores them into the database
            generateCodes.CreateDigitalCode(amount);

        }
    }
}
