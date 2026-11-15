using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Situation;

namespace SharboAPI.Endpoints;

public static class SituationEndpoint
{
    public static void MapSituationEndpoints(this IEndpointRouteBuilder routes) => MapSituationsApi(routes);

    private static async Task<IResult> GetAll(Guid groupId,
        ISituationService situationService, CancellationToken cancellationToken)
    {
        var result = await situationService.GetAllForGroupAsync(groupId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> GetById(Guid groupId, Guid situationId, ISituationService situationService,
        CancellationToken cancellationToken)
    {
        var result = await situationService.GetByIdAsync(groupId, situationId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> Create(Guid groupId, CreateSituationRequest request, 
        ISituationService situationService, CancellationToken cancellationToken)
    {
        var result = await situationService.AddAsync(groupId, request, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Created($"/api/groups/{groupId}/situations/{result.Value}", result.Value);
    }

    private static async Task<IResult> Update(Guid groupId, Guid situationId, UpdateSituationRequest request,
        ISituationService situationService, CancellationToken cancellationToken)
    {
        var result = await situationService.UpdateAsync(situationId, groupId, request, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> Delete(Guid groupId, Guid situationId, ISituationService situationService,
        CancellationToken cancellationToken)
    {
        var result = await situationService.DeleteAsync(groupId, situationId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    public static void MapSituationsApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/groups/{groupId:guid}/situations");

        group.MapGet("/", GetAll);
        group.MapGet("/{situationId:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPatch("/{situationId:guid}", Update);
        group.MapDelete("/{situationId:guid}", Delete);
    }
}
