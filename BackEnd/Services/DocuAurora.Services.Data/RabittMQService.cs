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

        public async Task<string> ReceiveResponse(string queue, IBasicProperties properties = null)
        {
            var consumer = new EventingBasicConsumer(this.channel);
            var tcs = new TaskCompletionSource<string>();

            consumer.Received += (model, ea) =>
            {
                var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                tcs.TrySetResult(response);
            };

            this.channel.BasicConsume(queue, autoAck: true, consumer);

            // Wait for the response or a timeout
            var responseTask = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(30)));

            if (responseTask == tcs.Task)
            {
                return tcs.Task.Result;
            }
            else
            {
                throw new TimeoutException("No response received from RabbitMQ.");
            }
        }

        public void SendMessage<T>(T message, string queue, string routingKey, string exchange, IBasicProperties properties)
        {
            var output = JsonSerializer.Serialize(message);

            properties = this.channel.CreateBasicProperties();
            properties.ReplyTo = "response";
            properties.CorrelationId = Guid.NewGuid().ToString();

            this.channel.BasicPublish(exchange, routingKey, properties, Encoding.UTF8.GetBytes(output));
        }
    }
}
