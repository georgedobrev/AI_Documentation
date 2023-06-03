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
        public void SendMessage<T>(string queue, string exchange, string routingKey, IBasicProperties basicProperties, T message);
    }
}
