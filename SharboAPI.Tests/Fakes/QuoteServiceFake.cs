using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Quote;

namespace SharboAPI.Tests.Fakes;

public class QuoteServiceFake(BehaviorFake behavior) : IQuoteService
{
    private readonly BehaviorFake _behavior = behavior;

    public Task<Result<IEnumerable<QuoteResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Success<IEnumerable<QuoteResult>>([]));
        }

        QuoteResult[] quoteResults = [
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now),
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now),
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now)
        ];

        return Task.FromResult(
            Result.Success<IEnumerable<QuoteResult>>(quoteResults));
    }

    public Task<Result<QuoteResult?>> GetByIdAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure<QuoteResult?>(Error.NotFound("no fake entity found")));
        }

        QuoteResult quoteResult =
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now);

        return Task.FromResult(
            Result.Success<QuoteResult?>(quoteResult));
    }

    public Task<Result<Guid?>> AddAsync(CreateQuoteRequest request, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure<Guid?>(Error.NotFound("no fake entity found")));
        }
        
        Guid createdQuoteId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");

        return Task.FromResult(
            Result.Success<Guid?>(createdQuoteId));
    }

    public Task<Result> DeleteAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure(Error.Forbidden("you're not allowed to perform this operation")));
        }

        return Task.FromResult(Result.Success());
    }

    public Task<Result> UpdateAsync(Guid quoteId, Guid groupId, UpdateQuoteRequest request, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure(Error.Forbidden("you're not allowed to perform this operation")));
        }

        return Task.FromResult(Result.Success());
    }
}
