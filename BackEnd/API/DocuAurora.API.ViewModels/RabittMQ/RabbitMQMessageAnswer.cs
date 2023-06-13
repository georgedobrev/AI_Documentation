using DocuAurora.API.ViewModels.User_Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels.RabittMQ
{
    public class RabbitMQMessageAnswer : IRabbitMQMessage<ModelAnswer>
    {
        public string CommandName { get; set; }
        public ModelAnswer Payload { get ; set ; }
    }
}
