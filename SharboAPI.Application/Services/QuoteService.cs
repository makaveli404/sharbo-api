using FluentValidation;
using Microsoft.AspNetCore.Http;
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
    IHttpContextAccessor httpContextAccessor) : IQuoteService
{
    public async Task<Result<IEnumerable<QuoteResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var quotes = await quoteRepository.GetAllByGroupIdAsync(groupId, cancellationToken);
        var quotesResult = quotes?.Select(quotes => new QuoteResult(
            quotes.Id,
            quotes.Text,
            quotes.CreatedById,
            quotes.LastModifiedById,
            quotes.CreationDate,
            quotes.LastModificationDate
        )) ?? [];

        return Result.Success(quotesResult);
    }

    public async Task<Result<QuoteResult?>> GetByIdAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        var quote = await quoteRepository.GetByIdAsync(quoteId, cancellationToken);

        if (quote is null)
        {
            return Result.Failure<QuoteResult?>(Error.NotFound($"No quote with ID: { quoteId } found"));
        }

        var quoteResult = new QuoteResult(
            quote.Id,
            quote.Text,
            quote.CreatedById,
            quote.LastModifiedById,
            quote.CreationDate,
            quote.LastModificationDate);

        return Result.Success<QuoteResult?>(quoteResult);
    }

    public async Task<Result<Guid?>> AddAsync(CreateQuoteRequest request, CancellationToken cancellationToken)
    {
        await createQuoteRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        // TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
        var requestingUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

        var groupParticipant = await groupParticipantRepository
            .GetByUserIdAndGroupIdAsync(requestingUserId, request.GroupId, cancellationToken);

        if (groupParticipant is null)
        {
            return Result.Failure<Guid?>(Error.NotFound("No participant found"));
        }

        var quote = Quote.Create(groupParticipant.Id, request.Text);
        var id = await quoteRepository.AddAsync(quote, cancellationToken);

        return Result.Success(id);
    }
    
    public async Task<Result> UpdateAsync(Guid quoteId, Guid groupId, UpdateQuoteRequest request, CancellationToken cancellationToken)
    {
        await updateQuoteRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        // TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
        var requestingUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

        var groupParticipant = await groupParticipantRepository
            .GetByUserIdAndGroupIdAsync(requestingUserId, groupId, cancellationToken);
        if (groupParticipant is null)
        {
            return Result.Failure<Result>(Error.NotFound("No participant found"));
        }

        var quote = await quoteRepository.GetByIdAsync(quoteId, cancellationToken);
        if (quote is null)
        {
            return Result.Failure<Result>(Error.NotFound($"No quote with ID: { quoteId } found"));
        }

        quote.UpdateText(groupParticipant.Id, request.Text);

        await quoteRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        var quote = await quoteRepository.GetByIdAsync(quoteId, cancellationToken);

        if (quote is null)
        {
            return Result.Failure(Error.NotFound($"No quote with ID: { quoteId } found"));
        }

        await quoteRepository.DeleteAsync(quote, cancellationToken);
        return Result.Success();
    }
}
