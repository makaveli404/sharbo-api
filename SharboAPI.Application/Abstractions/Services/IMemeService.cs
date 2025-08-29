using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Meme;

namespace SharboAPI.Application.Abstractions.Services;

public interface IMemeService
{
	Task<Result<Guid?>> AddAsync(CreateMemeRequest request, CancellationToken cancellationToken);
}
