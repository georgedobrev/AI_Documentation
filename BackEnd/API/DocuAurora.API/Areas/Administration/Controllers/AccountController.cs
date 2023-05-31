using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

using DocuAurora.API.ViewModels;
using DocuAurora.Data.Models;
using DocuAurora.API.ViewModels.Administration.Users;

using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System;

using Microsoft.IdentityModel.Tokens;

using System.Text;

using Microsoft.Extensions.Configuration;

using System.Configuration;
using System.Threading;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Auth.OAuth2;
using System.Net.Http;
using Microsoft.VisualBasic;
using NuGet.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using Google.Apis.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocuAurora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginUserViewModel model)
        {

            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            return Ok(result);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserViewModel model)
        {

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("GoogleLogin")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                $"client_id={_configuration["Authentication:Google:ClientId"]}" +
                $"&response_type=code" +
                $"&redirect_uri={_configuration["Authentication:Google:RedirectUri"]}" +
                $"&scope=openid%20email%20profile";
            return Redirect(redirectUrl);
        }

        [HttpGet]
        [Route("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse(string code)
        {
            using (var client = new HttpClient())
            {
                var req = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
                req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"code", code},
            {"client_id", _configuration["Authentication:Google:ClientId"]},
            {"client_secret", _configuration["Authentication:Google:ClientSecret"]},
            {"redirect_uri", _configuration["Authentication:Google:RedirectUri"]},
            {"grant_type", "authorization_code"}
        });

                var res = await client.SendAsync(req);
                var tokenResponse = await res.Content.ReadAsStringAsync();

                var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenResponse);
                string idToken = tokenData["id_token"];
                string accessToken = tokenData["access_token"];

                // Parse the id token to get user information
                var payload = GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings()).Result;

                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    // User doesn't exist, create a new user
                    user = new ApplicationUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email
                    };

                    var identityResult = await _userManager.CreateAsync(user);

                }
                var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            // Add more claims as needed
        };
                var token = GetToken(authClaims);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

        }

        [HttpPost("logout")]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Ok("Logout successful.");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddHours(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                // Don't reveal that the user does not exist or is not confirmed
                return Ok();

            // For more information on how to enable account confirmation and password reset please 
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            // Email the user the callback link here. You'll need to setup an email service for this.
            // await _emailSender.SendEmailAsync(model.Email, "Reset Password",
            //    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return Ok();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                // Don't reveal that the user does not exist
                return BadRequest();

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
    }
}
