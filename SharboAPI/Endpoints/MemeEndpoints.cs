using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Meme;

namespace SharboAPI.Endpoints;

public static class MemeEndpoints
{
    public static void MapMemeEndpoints(this IEndpointRouteBuilder routes)
    {
        MapMemesApi(routes);
    }
    
    private static async Task<IResult> CreateMeme(Guid groupId, CreateMemeRequest request, IMemeService memeService, 
        CancellationToken cancellationToken)
    {
        var result = await memeService.AddAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Created($"{request}/{result}", result);
    }

    private static void MapMemesApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/groups/{groupId:guid}/memes");

        group.MapPost("/", CreateMeme);
    }
}
