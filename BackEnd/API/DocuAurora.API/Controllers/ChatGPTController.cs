// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocuAurora.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using DocuAurora.Common;
    using DocuAurora.Services.Data.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Client;
    using Newtonsoft.Json;
    using OpenAI_API;
    using OpenAI_API.Completions;

    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = GlobalConstants.TrainerRoleName)]

    public class ChatGPTController : ControllerBase
    {
        private readonly IChatGPTService _chatGPTService;

        public ChatGPTController(IChatGPTService chatGPTService)
        {
            this._chatGPTService = chatGPTService;
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {

            var result = await this._chatGPTService.GenerateResponseChatGPT(value);
            return Ok(result);
        }

    }
}
