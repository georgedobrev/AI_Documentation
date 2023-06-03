using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IRabbitMQ
    {
        public void SendMessage<T>(string exchange, string routingKey, IBasicProperties basicProperties, T message);

        public void SetupUpQueue(string queue, string exchange, string routingKey);
    }
}
