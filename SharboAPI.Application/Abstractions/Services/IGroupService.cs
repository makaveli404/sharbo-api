using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Application.Abstractions.Services;

/// <summary>
/// Groups of the signed-in user. Every operation requires membership; a group the user does not belong to
/// is reported as not found.
/// </summary>
public interface IGroupService
{
	Task<Result<List<GroupResult>>> GetMineAsync(CancellationToken cancellationToken);
	Task<Result<GroupResult?>> GetById(Guid id, CancellationToken cancellationToken);
	Task<Result<Guid?>> AddAsync(CreateGroupRequest createGroupRequest, CancellationToken cancellationToken);
	Task<Result<GroupResult?>> UpdateAsync(Guid groupId, UpdateGroupRequest updatedGroupRequest, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
