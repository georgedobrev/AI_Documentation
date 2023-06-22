using DocuAurora.Services.Data.Contracts;
using Microsoft.Extensions.Configuration;
using OpenAI_API;
using OpenAI_API.Completions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly IConfiguration _configuration;

        public ChatGPTService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        public async Task<string> GenerateResumeJSONChatGPT(string inputText)
        {
            OpenAIAPI api = new OpenAIAPI(this._configuration["ChatGPTAPIkey"]);

            var chat = api.Chat.CreateConversation();

            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = inputText;
            completionRequest.Model = OpenAI_API.Models.Model.ChatGPTTurbo;

            chat.AppendSystemMessage("You are a writer.");

            chat.AppendUserInput(string.Format(this._configuration["ChatGPTtemplate"], inputText));

            string response = await chat.GetResponseFromChatbotAsync();

            return response;
        }

        public async Task<string> GenerateResponseChatGPT(string inputText)
        {
            OpenAIAPI api = new OpenAIAPI(this._configuration["ChatGPTAPIkey"]);

            var chat = api.Chat.CreateConversation();

            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = inputText;
            completionRequest.Model = OpenAI_API.Models.Model.ChatGPTTurbo;

            chat.AppendSystemMessage("You are a answer provider.");

            chat.AppendUserInput(string.Format(this._configuration["ChatGPTtemplateAsk"], inputText));

            string response = await chat.GetResponseFromChatbotAsync();

            return response;
        }
    }
}
