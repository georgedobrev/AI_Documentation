using DocuAurora.Services.Data.Configurations;
using DocuAurora.Services.Data.Contracts;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
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

        public RabittMQService(IModel channel)
        {
            this.channel = channel;
        }

        public void ReceiveResponse<T>(string queue, Action<T> action, IBasicProperties properties = null)
        {
          

            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += HandleMessageReceived;
            

            this.channel.BasicConsume(queue, autoAck: true, consumer: consumer);

            void HandleMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
            {
                
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var item = JsonSerializer.Deserialize<T>(message);
              
                Console.WriteLine("Reply received message: {0}", message);
                action(item);


            }
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
