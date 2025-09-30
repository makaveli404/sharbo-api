namespace SharboAPI.Application.DTO.Meme;

public sealed record UpdateMemeRequest(string ImagePath, string? Text);