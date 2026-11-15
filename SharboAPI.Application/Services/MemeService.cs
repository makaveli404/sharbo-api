using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Meme;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class MemeService(
	IMemeRepository memeRepository,
	IGroupParticipantRepository groupParticipantRepository,
	IValidator<CreateMemeRequest> createMemeRequestValidator,
	IValidator<UpdateMemeRequest> updateMemeRequestValidator,
	ICurrentUser currentUser) : IMemeService
{
	public async Task<Result<IEnumerable<MemeResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
	{
		if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
		{
			return Result.Failure<IEnumerable<MemeResult>>(GroupNotFound(groupId));
		}

		var memes = await memeRepository.GetAllByGroupIdAsync(groupId, cancellationToken);
		return Result.Success(memes.Select(ToMemeResult));
	}

	public async Task<Result<MemeResult?>> GetByIdAsync(Guid groupId, Guid memeId, CancellationToken cancellationToken)
	{
		if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
		{
			return Result.Failure<MemeResult?>(GroupNotFound(groupId));
		}

		var meme = await memeRepository.GetByIdAsync(groupId, memeId, cancellationToken);

		return meme is null
			? Result.Failure<MemeResult?>(MemeNotFound(memeId))
			: Result.Success<MemeResult?>(ToMemeResult(meme));
	}

	public async Task<Result<Guid?>> AddAsync(Guid groupId, CreateMemeRequest request, CancellationToken cancellationToken)
	{
		await createMemeRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

		var groupParticipant = await GetCurrentParticipantAsync(groupId, cancellationToken);
		if (groupParticipant is null)
		{
			return Result.Failure<Guid?>(GroupNotFound(groupId));
		}

		var meme = Meme.Create(groupParticipant.Id, request.ImagePath, request.Text);
		var id = await memeRepository.AddAsync(meme, cancellationToken);

		return Result.Success(id);
	}

	public async Task<Result> UpdateAsync(Guid memeId, Guid groupId, UpdateMemeRequest request, CancellationToken cancellationToken)
	{
		await updateMemeRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

		var groupParticipant = await GetCurrentParticipantAsync(groupId, cancellationToken);
		if (groupParticipant is null)
		{
			return Result.Failure(GroupNotFound(groupId));
		}

		var meme = await memeRepository.GetByIdAsync(groupId, memeId, cancellationToken);
		if (meme is null)
		{
			return Result.Failure(MemeNotFound(memeId));
		}

		meme.UpdateText(groupParticipant.Id, request.Text);

		await memeRepository.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}

	public async Task<Result> DeleteAsync(Guid groupId, Guid memeId, CancellationToken cancellationToken)
	{
		if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
		{
			return Result.Failure(GroupNotFound(groupId));
		}

		var meme = await memeRepository.GetByIdAsync(groupId, memeId, cancellationToken);
		if (meme is null)
		{
			return Result.Failure(MemeNotFound(memeId));
		}

		await memeRepository.DeleteAsync(meme, cancellationToken);
		return Result.Success();
	}

	/// <summary>
	/// The current user's participant in the group, or null when they are not a member (or the group does not exist;
	/// both are reported as "not found" so the API does not reveal other groups).
	/// </summary>
	private Task<GroupParticipant?> GetCurrentParticipantAsync(Guid groupId, CancellationToken cancellationToken)
		=> groupParticipantRepository.GetByUserIdAndGroupIdAsync(currentUser.UserId, groupId, cancellationToken);

	private static Error GroupNotFound(Guid groupId) => Error.NotFound($"No group with ID: { groupId } found");

	private static Error MemeNotFound(Guid memeId) => Error.NotFound($"No meme with ID: { memeId } found");

	private static MemeResult ToMemeResult(Meme meme) => new(
		meme.Id,
		meme.ImagePath,
		meme.Text,
		meme.CreatedById,
		meme.LastModifiedById,
		meme.CreationDate,
		meme.LastModificationDate);
}
