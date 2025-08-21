using BTG.Domain.Entities;
using System.Security.Claims;

namespace BTG.Application.Interfaces
{
    public interface ITokenProvider
    {
        (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(string userId, string role);
        RefreshToken GenerateRefreshToken();  
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        (string userId, string role) ReadRefreshToken(string token);
    }
}
