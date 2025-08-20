using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IGroupParticipantRepository
{
	Task<GroupParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<List<GroupParticipant>?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
	Task<GroupParticipant?> GetByUserIdAndGroupIdAsync(string userId, Guid groupId, CancellationToken cancellationToken);
    Task<Guid?> AddAsync(GroupParticipant groupParticipant, CancellationToken cancellationToken);
	Task DeleteAsync(Guid[] id, CancellationToken cancellationToken);
    Task DeleteRolesAsync(Guid participantId, List<Role> rolesToRemove, CancellationToken cancellationToken);
	Task SaveChangesAsync(CancellationToken cancellationToken);
}
