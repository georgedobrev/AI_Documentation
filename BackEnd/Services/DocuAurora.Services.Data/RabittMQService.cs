using DocuAurora.Services.Data.Contracts;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class RabittMQService : IRabbitMQService
    {
        public void SendMessage<T>(string queue,string exchange, string routingKey, IBasicProperties basicProperties, T message)
        {
            this.SetupUpQueue(queue, exchange, routingKey);
        }

        private void SetupUpQueue(string queue, string exchange, string routingKey)
        {
        }
    }
}
