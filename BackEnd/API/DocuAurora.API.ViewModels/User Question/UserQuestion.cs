using DocuAurora.API.ViewModels.User_Question.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels.Core_logic
{
    public class UserQuestion : IUserQuestion
    {
        public string InputQuestion { get; set; }
    }
}
