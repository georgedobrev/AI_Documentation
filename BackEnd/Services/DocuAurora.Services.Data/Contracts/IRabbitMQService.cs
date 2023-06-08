﻿using RabbitMQ.Client;
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
        public void SendMessage<T>(T message, string queue = "DocuAurora-Queue", string exchange = "DocuAurora-Exchange", string routingKey = "DocuAurora-api/RabittMQMessage", IBasicProperties properties = null);
    }
}