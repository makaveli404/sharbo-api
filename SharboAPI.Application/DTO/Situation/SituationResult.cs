namespace SharboAPI.Application.DTO.Situation;

public record SituationResult(Guid Id, string Text, Guid CreatedById,
    Guid LastModifiedById, DateTime CreationDate, DateTime LastModificationDate);
