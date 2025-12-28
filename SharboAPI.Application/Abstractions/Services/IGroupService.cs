using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Application.Abstractions.Services;

public interface IGroupService
{
	Task<Result<GroupResult?>> GetById(Guid id, CancellationToken cancellationToken);
	Task<Result<Guid?>> AddAsync(CreateGroupRequest createGroupRequest, CancellationToken cancellationToken);
	Task<Result<GroupResult?>> UpdateAsync(Guid groupId, UpdateGroupRequest updatedGroupRequest, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
