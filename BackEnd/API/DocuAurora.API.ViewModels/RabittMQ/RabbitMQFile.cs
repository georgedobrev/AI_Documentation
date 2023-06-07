using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels.RabittMQ
{
    public class RabbitMQFileKeyMessage : IRabbitMQMessage<string>
    {
        public string CommandName { get; set; }
        public string Payload { get; set; }
    }
}
