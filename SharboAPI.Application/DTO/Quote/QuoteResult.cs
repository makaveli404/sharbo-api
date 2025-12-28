namespace SharboAPI.Application.DTO.Quote;

public record QuoteResult(Guid Id, string Text, Guid CreatedById,
    Guid LastModifiedById, DateTime CreationDate, DateTime LastModificationDate);
