using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Meme;

namespace SharboAPI.Endpoints;

public static class MemeEndpoints
{
	public static void MapMemeEndpoints(this IEndpointRouteBuilder routes)
	{
		MapMemesApi(routes);
	}
    
	private static async Task<IResult> GetAll(Guid groupId, IMemeService memeService, CancellationToken cancellationToken)
	{
		var result = await memeService.GetAllAsync(cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest();
        }

		return TypedResults.Ok(result.Value);
    }

	private static async Task<IResult> GetById(Guid groupId, Guid memeId, IMemeService memeService, 
		CancellationToken cancellationToken)
	{
		var result = await memeService.GetByIdAsync(memeId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

		return TypedResults.Ok(result.Value);
    }

	private static async Task<IResult> Create(Guid groupId, CreateMemeRequest request, IMemeService memeService, 
		CancellationToken cancellationToken)
	{
		var result = await memeService.AddAsync(request, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.BadRequest();
		}

		return TypedResults.Created($"{request}/{result}", result);
	}

	private static async Task<IResult> Update(Guid groupId, Guid memeId, UpdateMemeRequest request, 
		IMemeService memeService, CancellationToken cancellationToken)
	{
		var result = await memeService.UpdateAsync(memeId, request, cancellationToken);

        if (result.IsFailure)
        {
			return TypedResults.BadRequest();
        }

		return TypedResults.NoContent();
    }

    private static async Task<IResult> Delete(Guid groupId, Guid memeId, IMemeService memeService, 
		CancellationToken cancellationToken)
    {
        var result = await memeService.DeleteAsync(memeId, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.NoContent();
    }

    private static void MapMemesApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/groups/{groupId:guid}/memes");

		group.MapGet("/", GetAll);
		group.MapGet("/{memeId:guid}", GetById);
		group.MapPost("/", Create);
		group.MapPut("/{memeId:guid}", Update);
		group.MapDelete("/{memeId:guid}", Delete);
	}
}
