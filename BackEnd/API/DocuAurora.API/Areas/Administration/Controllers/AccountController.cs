using DocuAurora.API.ViewModels.Administration.Users;
using DocuAurora.Data.Models;
using DocuAurora.Services.Data;
using DocuAurora.Services.Data.Contracts;
using DocuAurora.Services.Messaging;
using Google.Apis.Auth;
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
using System.Security.Cryptography;
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
        private readonly EmailService _emailService;
        private readonly IAuthService _authService;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailSender emailSender, EmailService emailService, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _emailService = emailService;
            _authService = authService;


        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
        {

            var result = await _emailService.RegisterUser(model.Username, model.Email, model.Password);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("A code must be supplied for email confirmation.");
            }

            var result = await _emailService.ConfirmEmail(token);

            if (result.Succeeded)
            {
                return Ok("Email confirmation succeeded.");
            }

            return BadRequest("Email confirmation failed.");
        }

        [HttpPost("Login")]
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
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _authService.GenerateJwtToken(authClaims);
                var refreshToken = await _authService.CreateRefreshToken(user);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    refreshToken
                });
            }
            return Unauthorized("Wrong password or username");
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

        [HttpPost]
        [Route("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse([FromBody] TokenModel tokenModel)
        {
            using var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string>

    {
        {"code", tokenModel.Token },
        {"client_id", _configuration["Authentication:Google:ClientId"]},
        {"client_secret", _configuration["Authentication:Google:ClientSecret"]},
        {"redirect_uri", _configuration["Authentication:Google:RedirectUri"]},
        {"grant_type", "authorization_code"}
    });

            var res = await client.SendAsync(req);
            var tokenResponse = await res.Content.ReadAsStringAsync();

            var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenResponse);
            string idToken = tokenData["id_token"];

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                };

                var identityResult = await _userManager.CreateAsync(user);
            }

            var result = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));

            await _signInManager.SignInAsync(user, isPersistent: false);

            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
    };
            var jwtToken = _authService.GenerateJwtToken(authClaims);
            var refreshToken = await _authService.CreateRefreshToken(user);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = jwtToken.ValidTo,
                refreshToken
            });
        }

        [HttpPost("Logout")]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Ok("Logout successful.");
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _emailService.SendPasswordResetEmail(model.Email);

            return result ? Ok() : BadRequest();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _emailService.ResetPassword(token, model.Password);

            return result.Succeeded ? Ok() : BadRequest(result.Errors.Select(e => e.Description));
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel model)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(model.JWTToken) as JwtSecurityToken;

            var userIdClaim = tokenS.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = userIdClaim.Value;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var storedRefreshToken = await _authService.GetStoredRefreshToken(user);
            if (storedRefreshToken != model.RefreshToken)
            {
                return Unauthorized();
            }

            var newJwtToken = _authService.GenerateJwtToken(tokenS.Claims.ToList());
            var newJwtTokenString = new JwtSecurityTokenHandler().WriteToken(newJwtToken);

            return Ok(new { token = newJwtTokenString });
        }

    }
}
