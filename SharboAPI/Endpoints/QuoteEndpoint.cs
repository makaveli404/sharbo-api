using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Quote;

namespace SharboAPI.Endpoints;

public static class QuoteEndpoint
{
    public static void MapQuoteEndpoints(this IEndpointRouteBuilder routes) => MapQuotesApi(routes);

    private static async Task<IResult> GetAll(Guid groupId, IQuoteService quoteService, 
        CancellationToken cancellationToken)
    {
        var result = await quoteService.GetAllForGroupAsync(groupId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> GetById(Guid groupId, Guid quoteId, IQuoteService quoteService,
        CancellationToken cancellationToken)
    {
        var result = await quoteService.GetByIdAsync(quoteId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> Create(Guid groupId, CreateQuoteRequest request,
        IQuoteService quoteService, CancellationToken cancellationToken)
    {
        var result = await quoteService.AddAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Created($"{request}/{result}", result);
    }

    private static async Task<IResult> Update(Guid groupId, Guid quoteId, UpdateQuoteRequest request,
        IQuoteService quoteService, CancellationToken cancellationToken)
    {
        var result = await quoteService.UpdateAsync(quoteId, groupId, request, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> Delete(Guid groupId, Guid quoteId, IQuoteService quoteService,
        CancellationToken cancellationToken)
    {
        var result = await quoteService.DeleteAsync(quoteId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.NoContent();
    }

    public static void MapQuotesApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/groups/{groupId:guid}/quotes");

        group.MapGet("/", GetAll);
        group.MapGet("/{quoteId:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPatch("/{quoteId:guid}", Update);
        group.MapDelete("/{quoteId:guid}", Delete);
    }
}
