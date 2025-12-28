namespace SharboAPI.Application.DTO.Meme;

public record MemeResult(Guid Id, string ImagePath, string? Text, Guid CreatedById, 
    Guid LastModifiedById, DateTime CreationDate, DateTime LastModificationDate);
