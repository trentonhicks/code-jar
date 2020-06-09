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
    public class PromoCodesController : ControllerBase
    {
        private readonly ILogger<PromoCodesController> _logger;
        private readonly IConfiguration _config;
        private readonly ICodeRepository _codeRepository;

        public PromoCodesController(ILogger<PromoCodesController> logger, IConfiguration config, ICodeRepository codeRepository)
        {
            _logger = logger;
            _config = config;
            _codeRepository = codeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string stringValue)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];

            var code = await _codeRepository.GetCodeAsync(stringValue, alphabet);

            var codeViewModel = new CodeViewModel
            {
                Id = code.Id,
                State = code.State.ToString(),
                StringValue = code.StringValue
            };

            return Ok(codeViewModel);
        }

        //Set code status to inactive
        [HttpDelete]
        public async Task<IActionResult> Deactivate([FromBody]string[] codes)
        {
            var alphabet = _config.GetSection("Base26")["alphabet"];
           
           foreach(var code in codes)
           {
               var codeFound = await _codeRepository.FindCodeBySeedValueAsync(code, alphabet);
               codeFound.Deactivate();
               await _codeRepository.UpdateCodeAsync(codeFound);
           }

           return Ok();
        }
    }
}
