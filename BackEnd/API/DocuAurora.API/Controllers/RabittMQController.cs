using DocuAurora.Services.Data.Contracts;
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
        public async Task<IActionResult> Post([FromBody] string value)
        {
            this._rabbitMQService.SendMessage(value);

            return Ok();
        }
    }
}
