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

        public PromoCodesController(ILogger<PromoCodesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Code> Get()
        {
            // Get the connection string from the appsetttings.json file
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("Storage");

            var sql = new SQL(connectionString);

            // Get the list of codes from the database
            var codes = sql.GetCodes();

            return codes;
        }
    }
}
