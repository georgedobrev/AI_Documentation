using DocuAurora.Services.Data.Contracts;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<RabbitMQOptions> _rabittOptions;
        private readonly ConnectionFactory _connectionFactory;
        private readonly Lazy<IConnection> _connectionCreation;
        private readonly Lazy<IModel> _channelCreation;

        public RabittMQService(IOptions<RabbitMQOptions> rabittOptions)
        {
            this._rabittOptions = rabittOptions;
            this._connectionFactory = new ConnectionFactory
            {
                //TO DO
            };

            this._connectionCreation = new Lazy<IConnection>(() => this._connectionFactory.CreateConnection());
            this._channelCreation = new Lazy<IModel>(() => Connection.CreateModel());
        }

        private IConnection Connection => this._connectionCreation.Value;
        private IModel Channel => this._channelCreation.Value;

        public void SendMessage<T>(string queue,string exchange, string routingKey, IBasicProperties basicProperties, T message)
        {
            this.SetupUpQueue(queue, exchange, routingKey);
        }

        private void SetupUpQueue(string queue, string exchange, string routingKey)
        {
        }
    }
}
