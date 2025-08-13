using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.GroupParticipant;

namespace SharboAPI.Application.Abstractions.Services;

public interface IGroupParticipantService
{
	Task<Result<GroupParticipantResult>> GetById(Guid id, CancellationToken cancellationToken);
	Task<Result<List<GroupParticipantResult>>> GetGroupParticipantsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
	Task<Result<Guid?>> AddAsync(CreateGroupParticipantRequest groupParticipantRequest, Guid groupId, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(List<Guid> ids, CancellationToken cancellationToken);
	Task<Result> UpdateRolesAsync(Guid participantId, UpdateGroupParticipantRolesRequest updateGroupParticipantRolesRequest, CancellationToken cancellationToken);
}
