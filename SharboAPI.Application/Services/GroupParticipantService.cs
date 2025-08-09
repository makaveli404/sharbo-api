using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.GroupParticipant;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public class GroupParticipantService(IGroupParticipantRepository groupParticipantRepository,
	IRoleRepository roleRepository) : IGroupParticipantService
{
	public async Task<Result<GroupParticipantResult?>> GetById(Guid id, CancellationToken cancellationToken)
	{
		var groupParticipant = await groupParticipantRepository.GetByIdAsync(id, cancellationToken);
		if (groupParticipant is null)
		{
			return Result.Failure<GroupParticipantResult?>(Error.NotFound("Group participant not found"));
		}

		return Result.Success<GroupParticipantResult?>(new GroupParticipantResult(groupParticipant.Id, groupParticipant.UserId));
	}

	public async Task<Result<Guid?>> AddAsync(CreateGroupParticipant createGroupParticipant, Guid groupId, CancellationToken cancellationToken)
	{
		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);

		if (participantRole is null)
		{
			return Result.Failure<Guid?>(Error.NotFound("Participant role not found"));
		}

		var groupParticipantRoles = new List<GroupParticipantRole>
		{
			GroupParticipantRole.Create(participantRole)
		};

		var groupParticipant = GroupParticipant.Create(groupId, createGroupParticipant.UserId, groupParticipantRoles);
		var result = await groupParticipantRepository.AddAsync(groupParticipant, cancellationToken);
		return Result.Success(result);
	}

	public async Task<Result> DeleteAsync(List<Guid> ids, CancellationToken cancellationToken)
	{
		await groupParticipantRepository.DeleteAsync(ids, cancellationToken);
		return Result.Success();
	}

	public Task<Result> SaveChangesAsync(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
