using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocuAurora.API.Controllers
{
    [Route("api/[controller]")]
    public class ChatGPTController : Controller
    {
        private readonly IConfiguration _configuration;

        public ChatGPTController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            OpenAIAPI api = new OpenAIAPI(this._configuration["ChatGPTAPIkey"]);

            var chat = api.Chat.CreateConversation();

            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = value;
            completionRequest.Model = OpenAI_API.Models.Model.ChatGPTTurbo;

            /// give instruction as System
            chat.AppendSystemMessage("You are a writer.");

            chat.AppendUserInput(string.Format(this._configuration["ChatGPTtemplate"], value));

            string response = await chat.GetResponseFromChatbotAsync();

            return Ok(response);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

