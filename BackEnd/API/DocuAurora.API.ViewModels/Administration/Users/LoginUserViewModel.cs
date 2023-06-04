using System.ComponentModel.DataAnnotations;

namespace DocuAurora.API.ViewModels.Administration.Users
{
    public class LoginUserViewModel
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
