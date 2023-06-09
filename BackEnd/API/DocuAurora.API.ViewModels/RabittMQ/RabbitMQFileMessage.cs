using System;
using System.Collections.Generic;
using DocuAurora.API.ViewModels.RabittMQ.Contracts;

namespace DocuAurora.API.ViewModels.RabittMQ
{
    public class RabbitMQFileMessage : IRabbitMQFileMessage<List<string>>
    {
        public string BucketName { get; set; }

        public List<string> DocumentNames { get; set; }
    }
}
