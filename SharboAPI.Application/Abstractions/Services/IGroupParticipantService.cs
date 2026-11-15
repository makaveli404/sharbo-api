using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.GroupParticipant;

namespace SharboAPI.Application.Abstractions.Services;

/// <summary>
/// Participants of a group. Every operation requires the current user to be a member of that group
/// and only touches participants of that same group.
/// </summary>
public interface IGroupParticipantService
{
	Task<Result<List<GroupParticipantResult>>> GetGroupParticipantsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
	Task<Result<List<Guid?>>> AddAsync(Guid groupId, string[] userIds, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid groupId, Guid[] participantIds, CancellationToken cancellationToken);
	Task<Result> UpdateRolesAsync(Guid groupId, Guid participantId, UpdateGroupParticipantRolesRequest updateGroupParticipantRolesRequest, CancellationToken cancellationToken);
}
