namespace SharboAPI.Application.DTO.Meme;

public sealed record CreateMemeRequest(string ImagePath, string? Text);
