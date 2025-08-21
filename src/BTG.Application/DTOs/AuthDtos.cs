namespace BTG.Application.DTOs;

public record RegisterRequest(string Username, string Password, string Role);
public record LoginRequest(string Username, string Password);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, string RefreshToken);
