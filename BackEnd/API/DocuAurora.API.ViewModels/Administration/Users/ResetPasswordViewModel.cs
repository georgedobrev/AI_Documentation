﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.ViewModels.Administration.Users
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Password { get; set; }
    }
}
