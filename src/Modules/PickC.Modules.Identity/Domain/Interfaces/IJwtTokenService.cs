using System.Security.Claims;

namespace PickC.Modules.Identity.Domain.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string userType, string mobileNo);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
