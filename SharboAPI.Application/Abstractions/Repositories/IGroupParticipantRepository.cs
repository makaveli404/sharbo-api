using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IGroupParticipantRepository
{
	Task<GroupParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<List<GroupParticipant>?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
	Task<Guid?> AddAsync(GroupParticipant groupParticipant, CancellationToken cancellationToken);
	Task DeleteAsync(Guid[] participantId, CancellationToken cancellationToken);
	Task DeleteRolesAsync(Guid participantId, List<RoleType> roleTypes, CancellationToken cancellationToken);
	Task SaveChangesAsync(CancellationToken cancellationToken);
}
