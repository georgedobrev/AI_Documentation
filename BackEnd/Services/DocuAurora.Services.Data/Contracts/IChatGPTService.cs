using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IChatGPTService
    {
        Task<string> GenerateResumeJSONChatGPT(string inputText);

        Task<string> GenerateResponseChatGPT(string inputText);
    }
}
