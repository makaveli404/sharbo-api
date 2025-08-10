using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Services;

public interface IGroupService
{
	Task<Result<GroupResult?>> GetById(Guid id, CancellationToken cancellationToken);
	Task<Result<Guid?>> AddAsync(CreateGroup createGroup, CancellationToken cancellationToken);
	Task<Result<GroupResult?>> UpdateAsync(Guid groupId, UpdateGroup updatedGroup, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
