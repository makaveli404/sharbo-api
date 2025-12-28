namespace SharboAPI.Application.DTO.Authentication;

public record LoginResult(string AccessToken, string RefreshToken, string ExpiresIn);
