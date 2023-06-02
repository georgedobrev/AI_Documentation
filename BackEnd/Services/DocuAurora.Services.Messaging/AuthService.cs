using DocuAurora.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Messaging
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AuthService(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<bool> SendPasswordResetEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Id};{code}"));

            var callbackUrl = $"https://localhost:44319/resetpassword?token={token}"; /* Generate URL to frontend page with token as query parameter */
            var message = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";
            await _emailSender.SendEmailAsync("vladislav.milchov.work@gmail.com", "Vladislav", email, "Reset Password", message);

            return true;
        }

        public async Task<IdentityResult> ResetPassword(string token, string newPassword)
        {
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(";");
            var user = await _userManager.FindByIdAsync(credentials[0]);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var result = await _userManager.ResetPasswordAsync(user, credentials[1], newPassword);

            return result;
        }
    }
}