using DocuAurora.API.ViewModels;
using DocuAurora.API.ViewModels.RabittMQ;
using DocuAurora.Services.Data.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace DocuAurora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabittMQController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IConfiguration _configuration;

        public RabittMQController(
            IRabbitMQService rabbitMQService,
            IConfiguration configuration)
        {
            this._rabbitMQService = rabbitMQService;
            this._configuration = configuration;
        }

        [HttpPost("message")]
        public async Task<IActionResult> Post([FromBody] RabbitMQMessage message)
        {
            this._rabbitMQService.SendMessage(message);

            return Ok();
        }

        [HttpPost("filemessage")]
        public async Task<IActionResult> Post([FromBody] RabbitMQFileMessage message)
        {
            this._rabbitMQService.SendMessage(message,routingKey: this._configuration["RabbitMQRoutingKeyFileConfiguration:RoutingKey"]);

            return Ok();
        }

        // 1. FileController - Get all documents + bucketname // multitenant - autobucketname
        // 2. RabbitControler - Get documentNames[] + bucketname
        //      * Get keys / Use the name (id for the document)
        //      * Send message bucketname + documentNames[] - json - byte[]
        //      * CommandName - Embed-Documents / logging 
        //          p. 
    }
}
