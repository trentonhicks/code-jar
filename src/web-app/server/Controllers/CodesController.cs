using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeJar.Domain;
using CodeJar.Infrastructure;
using CodeJar.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeJar.WebApp.Controllers
{
    [ApiController]
    [Route("codes")]
    public class CodesController : ControllerBase
    {
        private readonly ILogger<CodesController> _logger;
        private readonly IConfiguration _config;
        private readonly ICodeRepository _codeRepository;

        public CodesController(ILogger<CodesController> logger, IConfiguration config, ICodeRepository codeRepository)
        {
            _logger = logger;
            _config = config;
            _codeRepository = codeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string stringValue)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var seedValue = CodeConverter.ConvertFromCode(stringValue, alphabet);
            var code = await _codeRepository.GetCodeAsync(seedValue);

            var codeViewModel = new CodeViewModel
            {
                Id = code.Id,
                State = code.State,
                StringValue = CodeConverter.ConvertToCode(seedValue, alphabet)
            };

            return Ok(codeViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> Deactivate([FromBody]string[] codes)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];

            foreach(var code in codes)
            {
                var seedValue = CodeConverter.ConvertFromCode(code, alphabet);
                var codeToDeactivate = await _codeRepository.GetCodeForDeactivationAsync(seedValue);
                codeToDeactivate.Deactivate();
                await _codeRepository.UpdateCodeAsync(codeToDeactivate);
            }

            return Ok();
        }

        [HttpPost("redeem-code")]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
            var seedValue = CodeConverter.ConvertFromCode(value, alphabet);
            var code = await _codeRepository.GetCodeForRedemptionAsync(seedValue);

            code.Redeem();

            await _codeRepository.UpdateCodeAsync(code);

            return Ok();
        }
    }
}
