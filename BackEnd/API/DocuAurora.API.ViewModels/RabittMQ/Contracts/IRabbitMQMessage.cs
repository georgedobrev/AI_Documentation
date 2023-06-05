using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels
{
    public interface IRabbitMQMessage<T>
    {
        public string CommandName { get; set; }

        public T Payload { get; set; }
    }
}
