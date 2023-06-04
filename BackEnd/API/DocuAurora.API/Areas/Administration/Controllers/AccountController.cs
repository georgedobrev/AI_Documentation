using DocuAurora.API.ViewModels.Administration.Users;
using DocuAurora.Data.Models;
using DocuAurora.Services.Messaging;
using Google.Apis.Auth;
using Google.Apis.Oauth2.v2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailSender emailSender, AuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _authService = authService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
        {

            var result = await _authService.RegisterUser(model.Username, model.Email, model.Password);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("A code must be supplied for email confirmation.");

            var result = await _authService.ConfirmEmail(token);

            if (result.Succeeded)
            {
                return Ok("Email confirmation succeeded.");
            }

            return BadRequest("Email confirmation failed.");
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
            if (!ModelState.IsValid) return BadRequest();

            var result = await _authService.SendPasswordResetEmail(model.Email);

            return result ? Ok() : BadRequest();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromQuery]string token, [FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _authService.ResetPassword(token, model.Password);

            return result.Succeeded ? Ok() : BadRequest(result.Errors.Select(e => e.Description));
        }

    }
}
