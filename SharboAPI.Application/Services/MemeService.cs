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
	IHttpContextAccessor httpContextAccessor) : IMemeService
{
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

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Meme>>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Meme?>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateAsync(Guid id, UpdateMemeRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
