using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IGroupParticipantRepository
{
	Task<GroupParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Guid?> AddAsync(GroupParticipant groupParticipant, CancellationToken cancellationToken);
	Task DeleteAsync(List<Guid> id, CancellationToken cancellationToken);
	Task SaveChangesAsync(CancellationToken cancellationToken);
}
