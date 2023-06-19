using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels.Administration.Users
{
    public class RefreshTokenViewModel
    {
        public string JWTToken { get; set; }

        public string RefreshToken { get; set; }

    }
}
