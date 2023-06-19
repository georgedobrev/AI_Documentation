using DocuAurora.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Contracts
{
    public interface IAuthService
    {
        JwtSecurityToken GenerateJwtToken(List<Claim> claims);

        Task<string> CreateRefreshToken(ApplicationUser user);

        Task<string> GetStoredRefreshToken(ApplicationUser user);

        Task RemoveRefreshToken(ApplicationUser user);
    }

}