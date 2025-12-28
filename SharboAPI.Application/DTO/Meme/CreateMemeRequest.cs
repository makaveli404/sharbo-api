namespace SharboAPI.Application.DTO.Meme;

public sealed record CreateMemeRequest(Guid GroupId, string ImagePath, string? Text);
