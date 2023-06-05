using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IRabbitMQService
    {
        public void SendMessage<T>(T message, string queue = "DocuAurora-queue", string exchange = "DocuAurora-exchange", string routingKey = "DocuAurora-api/RabittMQ", IBasicProperties properties = null);
    }
}
