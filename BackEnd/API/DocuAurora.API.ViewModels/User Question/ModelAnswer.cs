using DocuAurora.API.ViewModels.User_Question.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels.User_Question
{
    public class ModelAnswer : IUserQuestion
    {
        public string InputQuestion { get ; set ; }
        public string Answer { get; set ; }
    }
}
