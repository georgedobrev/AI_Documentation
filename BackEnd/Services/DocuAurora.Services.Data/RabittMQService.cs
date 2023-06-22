using DocuAurora.API.ViewModels.RabittMQ;
using DocuAurora.Services.Data.Configurations;
using DocuAurora.Services.Data.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class RabittMQService : IRabbitMQService
    {
        private readonly IModel channel;
        private readonly ConcurrentDictionary<string, EventingBasicConsumer> _consumer;
        private readonly ConcurrentDictionary<string, BlockingCollection<object>> _blockingCollection;

        public RabittMQService(IModel channel)
        {
            this.channel = channel;
            this._consumer = new ConcurrentDictionary<string, EventingBasicConsumer>();
            this._blockingCollection = new ConcurrentDictionary<string, BlockingCollection<object>>();
        }

        public T ReceiveResponse<T>(string queue, IBasicProperties properties = null)
        {
            if (!this._consumer.ContainsKey(queue))
            {
                var consumer = new EventingBasicConsumer(this.channel);
                consumer.Received += HandleMessageReceived;

                this.channel.BasicConsume(queue, autoAck: true, consumer: consumer);

                this._consumer[queue] = consumer;
                this._blockingCollection[queue] = new BlockingCollection<object>();
            }

            void HandleMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var item = JsonSerializer.Deserialize<T>(message);

                this._blockingCollection[queue].Add(item);
            }

            if (this._blockingCollection[queue].TryTake(out var item, TimeSpan.FromSeconds(60)))
            {
                return (T)item;
            }

            return default;
        }

        public void SendMessage<T>(T message, string queue, string routingKey, string exchange, IBasicProperties properties)
        {
            var output = JsonSerializer.Serialize(message);

            properties = this.channel.CreateBasicProperties();
            properties.ReplyTo = "response";

            this.channel.BasicPublish(exchange, routingKey, properties, Encoding.UTF8.GetBytes(output));
        }
    }
}
