using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Meme;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Services;

public interface IMemeService
{
	Task<Result<IEnumerable<Meme>>> GetAllAsync(CancellationToken cancellationToken);
	Task<Result<Meme?>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Result<Guid?>> AddAsync(CreateMemeRequest request, CancellationToken cancellationToken);
	Task<Result> UpdateAsync(Guid id, UpdateMemeRequest request, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
