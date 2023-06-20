using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IRabbitMQService
    {
        void SendMessage<T>(T message, string queue, string routingKey, string exchange = "DocuAurora-Exchange",  IBasicProperties properties = null);
       void ReceiveResponse<T>(string queue, Action<T> action, IBasicProperties properties = null);
    }
}