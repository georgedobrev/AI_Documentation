using DocuAurora.API.ViewModels.Core_logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels.RabittMQ
{
    public class RabbitMQMessage : IRabbitMQMessage<UserQuestion>
    {
        public string CommandName { get; set; }
        public UserQuestion Payload { get; set; }
    }
}
