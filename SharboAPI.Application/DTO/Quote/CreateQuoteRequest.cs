namespace SharboAPI.Application.DTO.Quote;

public record CreateQuoteRequest(Guid GroupId, string Text);
