using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Situation;

namespace SharboAPI.Application.Abstractions.Services;

public interface ISituationService
{
    Task<Result<IEnumerable<SituationResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Result<SituationResult?>> GetByIdAsync(Guid situationId, CancellationToken cancellationToken);
    Task<Result<Guid?>> AddAsync(CreateSituationRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Guid situationId, Guid groupId, UpdateSituationRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid situationId, CancellationToken cancellationToken);
}
