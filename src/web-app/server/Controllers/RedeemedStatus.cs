using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeJar.Domain;
using CodeJar.Infrastructure;
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
        private readonly ICodeRepository _codeRepository;

        public RedeemdedStatus(ILogger<RedeemdedStatus> logger, IConfiguration config, ICodeRepository codeRepository)
        {
            _logger = logger;
            _config = config;
            _codeRepository = codeRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string code)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var codeFound = await _codeRepository.FindCodeBySeedValueAsync(code, alphabet);
            
            codeFound.Redeem();

            await _codeRepository.UpdateCodeAsync(codeFound);

            return Ok();
        }
    }
}
