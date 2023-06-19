using DocuAurora.Data.Models;
using DocuAurora.Services.Data.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public JwtSecurityToken GenerateJwtToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            return new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddHours(2),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        public async Task<string> CreateRefreshToken(ApplicationUser user)
        {
            var refreshToken = GenerateRefreshToken();
            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "RefreshToken", refreshToken);
            return refreshToken;
        }

        public async Task<string> GetStoredRefreshToken(ApplicationUser user)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, "JWT", "RefreshToken");
        }

        public async Task RemoveRefreshToken(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "JWT", "RefreshToken");
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
