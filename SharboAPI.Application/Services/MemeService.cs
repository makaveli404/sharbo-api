using SharboAPI.Domain.Models;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Meme;
using SharboAPI.Application.Common.Errors;
using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace SharboAPI.Application.Services;

public sealed class MemeService(
	IMemeRepository memeRepository, 
	IGroupParticipantRepository groupParticipantRepository,
	IValidator<CreateMemeRequest> createMemeRequestValidator, 
	IValidator<UpdateMemeRequest> updateMemeRequestValidator, 
	IHttpContextAccessor httpContextAccessor) : IMemeService
{
    public async Task<Result<IEnumerable<MemeResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var memesForGroup = await memeRepository.GetAllByGroupIdAsync(groupId, cancellationToken);
        var memeResults = memesForGroup.Select(meme => new MemeResult(
            meme.Id,
            meme.ImagePath,
            meme.Text,
            meme.CreatedById,
            meme.LastModifiedById,
            meme.CreationDate,
            meme.LastModificationDate));

		return Result.Success(memeResults);
    }

    public async Task<Result<MemeResult?>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
		var meme = await memeRepository.GetByIdAsync(id, cancellationToken);

        if (meme is null)
        {
			return Result.Failure<MemeResult?>(Error.NotFound($"No meme with ID: { id } found"));
        }

		var memeResult = new MemeResult(
            meme.Id,
            meme.ImagePath,
            meme.Text,
            meme.CreatedById,
            meme.LastModifiedById,
            meme.CreationDate,
            meme.LastModificationDate);

		return Result.Success<MemeResult?>(memeResult);
    }

	public async Task<Result<Guid?>> AddAsync(CreateMemeRequest request, CancellationToken cancellationToken)
	{
		await createMemeRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

		// TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
		var requestingUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2"; 

		var groupParticipant = await groupParticipantRepository
			.GetByUserIdAndGroupIdAsync(requestingUserId, request.GroupId, cancellationToken);

		if (groupParticipant is null)
		{
			return Result.Failure<Guid?>(Error.NotFound("No participant found"));
		}

		var meme = Meme.Create(
			groupParticipant.Id,
			request.ImagePath,
			request.Text
		);
        
		var id = await memeRepository.AddAsync(meme, cancellationToken);
		return Result.Success(id);
	}
    
	public async Task<Result> UpdateAsync(Guid id, Guid groupId, UpdateMemeRequest request, CancellationToken cancellationToken)
    {
        await updateMemeRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        // TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
        var requestingUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

        var groupParticipant = await groupParticipantRepository
            .GetByUserIdAndGroupIdAsync(requestingUserId, groupId, cancellationToken);
        if (groupParticipant is null)
        {
            return Result.Failure<Result>(Error.NotFound("No participant found"));
        }

        var meme = await memeRepository.GetByIdAsync(id, cancellationToken);
        if (meme is null)
        {
            return Result.Failure<Result>(Error.NotFound($"No meme with ID: { id } found"));
        }

        meme.UpdateText(groupParticipant.Id, request.Text);

        await memeRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var meme = await memeRepository.GetByIdAsync(id, cancellationToken);

        if (meme is null)
        {
            return Result.Failure<Result>(Error.NotFound($"No meme with ID: { id } found"));
        }

        await memeRepository.DeleteAsync(meme, cancellationToken);
        return Result.Success();
    }
}
