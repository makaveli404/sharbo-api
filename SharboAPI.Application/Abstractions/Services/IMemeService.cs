using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Meme;

namespace SharboAPI.Application.Abstractions.Services;

public interface IMemeService
{
	Task<Result<IEnumerable<MemeResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken);
	Task<Result<MemeResult?>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Result<Guid?>> AddAsync(CreateMemeRequest request, CancellationToken cancellationToken);
	Task<Result> UpdateAsync(Guid id, Guid groupId, UpdateMemeRequest request, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
