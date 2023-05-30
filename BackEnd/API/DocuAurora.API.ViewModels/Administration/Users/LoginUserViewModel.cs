using System.ComponentModel.DataAnnotations;

namespace DocuAurora.API.ViewModels.Administration.Users
{
    public class LoginUserViewModel
    {
       
        private string  id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string Email { get; set; }
    }
}
