namespace SharboAPI.Application.DTO.User;

public sealed record CreateUserRequest(string Nickname, string Email, string Password);
