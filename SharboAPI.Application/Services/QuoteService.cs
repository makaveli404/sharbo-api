using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Quote;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class QuoteService(
    IQuoteRepository quoteRepository,
    IGroupParticipantRepository groupParticipantRepository,
    IValidator<CreateQuoteRequest> createQuoteRequestValidator,
    IValidator<UpdateQuoteRequest> updateQuoteRequestValidator,
    ICurrentUser currentUser) : IQuoteService
{
    public async Task<Result<IEnumerable<QuoteResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
        {
            return Result.Failure<IEnumerable<QuoteResult>>(GroupNotFound(groupId));
        }

        var quotes = await quoteRepository.GetAllByGroupIdAsync(groupId, cancellationToken);
        var quotesResult = quotes?.Select(ToQuoteResult) ?? [];

        return Result.Success(quotesResult);
    }

    public async Task<Result<QuoteResult?>> GetByIdAsync(Guid groupId, Guid quoteId, CancellationToken cancellationToken)
    {
        if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
        {
            return Result.Failure<QuoteResult?>(GroupNotFound(groupId));
        }

        var quote = await quoteRepository.GetByIdAsync(groupId, quoteId, cancellationToken);

        if (quote is null)
        {
            return Result.Failure<QuoteResult?>(QuoteNotFound(quoteId));
        }

        return Result.Success<QuoteResult?>(ToQuoteResult(quote));
    }

    public async Task<Result<Guid?>> AddAsync(Guid groupId, CreateQuoteRequest request, CancellationToken cancellationToken)
    {
        await createQuoteRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var groupParticipant = await GetCurrentParticipantAsync(groupId, cancellationToken);
        if (groupParticipant is null)
        {
            return Result.Failure<Guid?>(GroupNotFound(groupId));
        }

        var quote = Quote.Create(groupParticipant.Id, request.Text);
        var id = await quoteRepository.AddAsync(quote, cancellationToken);

        return Result.Success(id);
    }

    public async Task<Result> UpdateAsync(Guid quoteId, Guid groupId, UpdateQuoteRequest request, CancellationToken cancellationToken)
    {
        await updateQuoteRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var groupParticipant = await GetCurrentParticipantAsync(groupId, cancellationToken);
        if (groupParticipant is null)
        {
            return Result.Failure(GroupNotFound(groupId));
        }

        var quote = await quoteRepository.GetByIdAsync(groupId, quoteId, cancellationToken);
        if (quote is null)
        {
            return Result.Failure(QuoteNotFound(quoteId));
        }

        quote.UpdateText(groupParticipant.Id, request.Text);

        await quoteRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid groupId, Guid quoteId, CancellationToken cancellationToken)
    {
        if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
        {
            return Result.Failure(GroupNotFound(groupId));
        }

        var quote = await quoteRepository.GetByIdAsync(groupId, quoteId, cancellationToken);

        if (quote is null)
        {
            return Result.Failure(QuoteNotFound(quoteId));
        }

        await quoteRepository.DeleteAsync(quote, cancellationToken);
        return Result.Success();
    }

    /// <summary>
    /// Returns the current user's participant in the group, or null when the user is not a member
    /// (or the group does not exist; both are reported as "not found" so the API does not reveal other groups).
    /// </summary>
    private Task<GroupParticipant?> GetCurrentParticipantAsync(Guid groupId, CancellationToken cancellationToken)
        => groupParticipantRepository.GetByUserIdAndGroupIdAsync(currentUser.UserId, groupId, cancellationToken);

    private static Error GroupNotFound(Guid groupId) => Error.NotFound($"No group with ID: { groupId } found");

    private static Error QuoteNotFound(Guid quoteId) => Error.NotFound($"No quote with ID: { quoteId } found");

    private static QuoteResult ToQuoteResult(Quote quote) => new(
        quote.Id,
        quote.Text,
        quote.CreatedById,
        quote.LastModifiedById,
        quote.CreationDate,
        quote.LastModificationDate);
}
