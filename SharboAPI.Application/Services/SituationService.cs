using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Situation;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class SituationService(
    ISituationRepository situationRepository, 
    IGroupParticipantRepository groupParticipantRepository,
    IValidator<CreateSituationRequest> createSituationRequestValidator,
    IValidator<UpdateSituationRequest> updateSituationRequestValidator) : ISituationService
{
    public async Task<Result<IEnumerable<SituationResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        var situations = await situationRepository.GetAllByGroupIdAsync(groupId, cancellationToken);
        var situationsResult = situations.Select(situation => new SituationResult(
            situation.Id,
            situation.Text,
            situation.CreatedById,
            situation.LastModifiedById,
            situation.CreationDate,
            situation.LastModificationDate
        ));

        return Result.Success(situationsResult);
    }

    public async Task<Result<SituationResult?>> GetByIdAsync(Guid situationId, CancellationToken cancellationToken)
    {
        var situation = await situationRepository.GetByIdAsync(situationId, cancellationToken);

        if (situation is null)
        {
            return Result.Failure<SituationResult?>(Error.NotFound($"No situation with ID: { situationId } found"));
        }

        var situationResult = new SituationResult(
            situation.Id,
            situation.Text,
            situation.CreatedById,
            situation.LastModifiedById,
            situation.CreationDate,
            situation.LastModificationDate);

        return Result.Success<SituationResult?>(situationResult);
    }

    public async Task<Result<Guid?>> AddAsync(CreateSituationRequest request, CancellationToken cancellationToken)
    {
        await createSituationRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        // TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
        var requestingUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

        var groupParticipant = await groupParticipantRepository
            .GetByUserIdAndGroupIdAsync(requestingUserId, request.GroupId, cancellationToken);

        if (groupParticipant is null)
        {
            return Result.Failure<Guid?>(Error.NotFound("No participant found"));
        }

        var situation = Situation.Create(groupParticipant.Id, request.Text);
        var id = await situationRepository.AddAsync(situation, cancellationToken);

        return Result.Success(id);
    }
    
    public async Task<Result> UpdateAsync(Guid situationId, Guid groupId, UpdateSituationRequest request, 
        CancellationToken cancellationToken)
    {
        await updateSituationRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        // TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
        var requestingUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

        var groupParticipant = await groupParticipantRepository
            .GetByUserIdAndGroupIdAsync(requestingUserId, groupId, cancellationToken);
        if (groupParticipant is null)
        {
            return Result.Failure<Result>(Error.NotFound("No participant found"));
        }

        var situation = await situationRepository.GetByIdAsync(situationId, cancellationToken);
        if (situation is null)
        {
            return Result.Failure<Result>(Error.NotFound($"No situation with ID: { situationId } found"));
        }

        situation.UpdateText(groupParticipant.Id, request.Text);
        
        await situationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid situationId, CancellationToken cancellationToken)
    {
        var situation = await situationRepository.GetByIdAsync(situationId, cancellationToken);

        if (situation is null)
        {
            return Result.Failure(Error.NotFound($"No situation with ID: { situationId } found"));
        }

        await situationRepository.DeleteAsync(situation, cancellationToken);
        return Result.Success();
    }
}
