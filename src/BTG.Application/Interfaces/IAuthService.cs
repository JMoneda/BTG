using BTG.Application.DTOs;

namespace BTG.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct);
    Task<AuthResponse> RefreshAsync(RefreshRequest req, CancellationToken ct);
    Task RevokeAsync(string userId, string? refreshToken, CancellationToken ct);
}
