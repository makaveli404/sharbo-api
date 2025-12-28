using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.GroupParticipant;

namespace SharboAPI.Application.Abstractions.Services;

public interface IGroupParticipantService
{
	Task<Result<GroupParticipantResult>> GetById(Guid id, CancellationToken cancellationToken);
	Task<Result<List<GroupParticipantResult>>> GetGroupParticipantsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
	Task<Result<List<Guid?>>> AddAsync(Guid groupId, string[] userIds, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid[] participantIds, CancellationToken cancellationToken);
	Task<Result> UpdateRolesAsync(Guid participantId, UpdateGroupParticipantRolesRequest updateGroupParticipantRolesRequest, CancellationToken cancellationToken);
}
