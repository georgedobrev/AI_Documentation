using DocuAurora.Services.Data.Configurations;
using DocuAurora.Services.Data.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ude;

namespace DocuAurora.Services.Data
{
    public class RabittMQService : IRabbitMQService
    {
        private readonly IModel channel;

        public RabittMQService(IModel channel)
        {
            this.channel = channel;
        }

        public void SendFile(IFormFile file, string queue, string exchange, string routingKey, IBasicProperties properties)
        {
            var encoding = DetectEncoding(file);

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] fileContent = memoryStream.ToArray();
                string fb = string.Join(" ", fileContent);
                var output = JsonSerializer.Serialize(new { encoding, fb});
                this.channel.BasicPublish(exchange, routingKey, properties, Encoding.UTF8.GetBytes(output));

            }
        }

        public string DetectEncoding(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var detector = new CharsetDetector();
                detector.Feed(reader.BaseStream);
                detector.DataEnd();
                if (detector.Charset != null)
                {
                    return detector.Charset;
                }
            }

            return "Unknown";
        }
            public void SendMessage<T>(T message, string queue, string exchange, string routingKey, IBasicProperties properties)
        {
            var output = JsonSerializer.Serialize(message);

            this.channel.BasicPublish(exchange, routingKey, properties, Encoding.UTF8.GetBytes(output));
        }
    }
}
