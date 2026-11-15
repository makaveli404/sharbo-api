using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Meme;

namespace SharboAPI.Application.Abstractions.Services;

public interface IMemeService
{
	Task<Result<IEnumerable<MemeResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken);
	Task<Result<MemeResult?>> GetByIdAsync(Guid groupId, Guid memeId, CancellationToken cancellationToken);
	Task<Result<Guid?>> AddAsync(Guid groupId, CreateMemeRequest request, CancellationToken cancellationToken);
	Task<Result> UpdateAsync(Guid id, Guid groupId, UpdateMemeRequest request, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid groupId, Guid memeId, CancellationToken cancellationToken);
}
