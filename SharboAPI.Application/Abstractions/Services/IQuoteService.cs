using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Quote;

namespace SharboAPI.Application.Abstractions.Services;

public interface IQuoteService
{
    Task<Result<IEnumerable<QuoteResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Result<QuoteResult?>> GetByIdAsync(Guid quoteId, CancellationToken cancellationToken);
    Task<Result<Guid?>> AddAsync(CreateQuoteRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Guid quoteId, Guid groupId, UpdateQuoteRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid quoteId, CancellationToken cancellationToken);
}
