using DocuAurora.Services.Data.Configurations;
using DocuAurora.Services.Data.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class RabittMQService : IRabbitMQService
    {
        private readonly IOptions<RabbitMQOptions> _rabittOptions;
        private readonly ConnectionFactory _connectionFactory;
        private readonly Lazy<IConnection> _connectionCreation;
        private readonly Lazy<IModel> _channelCreation;

        public RabittMQService(IOptions<RabbitMQOptions> rabittOptions)
        {
            this._rabittOptions = rabittOptions;
            this._connectionFactory = new ConnectionFactory
            {
               HostName = this._rabittOptions.Value.HostName,
            };

            this._connectionCreation = new Lazy<IConnection>(() => this._connectionFactory.CreateConnection());
            this._channelCreation = new Lazy<IModel>(() => this.Connection.CreateModel());
        }

        private IConnection Connection => this._connectionCreation.Value;
        private IModel Channel => this._channelCreation.Value;

        public void SendMessage<T>(string queue, string exchange, string routingKey, T message, IBasicProperties properties = null)
        {
            this.SetupUpQueue(queue, exchange, routingKey);

            var output = JsonSerializer.Serialize(message);

            this.Channel.BasicPublish(exchange, routingKey, properties, Encoding.UTF8.GetBytes(output));
        }

        private void SetupUpQueue(string queue, string exchange, string routingKey)
        {
            this.Channel.ExchangeDeclare(exchange, ExchangeType.Direct);

            this.Channel.QueueDeclare(
               queue,
               durable: false,
               exclusive: false,
               autoDelete: false,
               arguments: null);

            this.Channel.QueueBind(queue,exchange,routingKey);
        }
    }
}
