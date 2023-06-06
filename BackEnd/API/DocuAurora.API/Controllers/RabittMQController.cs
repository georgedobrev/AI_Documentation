using DocuAurora.API.ViewModels;
using DocuAurora.API.ViewModels.RabittMQ;
using DocuAurora.Services.Data.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocuAurora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabittMQController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;

        public RabittMQController(IRabbitMQService rabbitMQService)
        {
            this._rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RabbitMQMessage message)
        {
            this._rabbitMQService.SendMessage(message);

            return Ok();
        }

        [HttpPost("SendFile")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            this._rabbitMQService.SendFile(file);

            return Ok();
        }
    }
}
