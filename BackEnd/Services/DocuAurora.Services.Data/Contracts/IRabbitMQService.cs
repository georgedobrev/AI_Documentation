namespace DocuAurora.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using RabbitMQ.Client;

    public interface IRabbitMQService
    {
        void SendMessage<T>(T message, string queue, string routingKey, string exchange = "DocuAurora-Exchange",  IBasicProperties properties = null);

        Task ReceiveResponse<T>(string queue, IBasicProperties properties = null);
    }
}
