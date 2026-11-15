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
    IValidator<UpdateSituationRequest> updateSituationRequestValidator,
    ICurrentUser currentUser) : ISituationService
{
    public async Task<Result<IEnumerable<SituationResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
        {
            return Result.Failure<IEnumerable<SituationResult>>(GroupNotFound(groupId));
        }

        var situations = await situationRepository.GetAllByGroupIdAsync(groupId, cancellationToken);
        return Result.Success(situations.Select(ToSituationResult));
    }

    public async Task<Result<SituationResult?>> GetByIdAsync(Guid groupId, Guid situationId, CancellationToken cancellationToken)
    {
        if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
        {
            return Result.Failure<SituationResult?>(GroupNotFound(groupId));
        }

        var situation = await situationRepository.GetByIdAsync(groupId, situationId, cancellationToken);

        return situation is null
            ? Result.Failure<SituationResult?>(SituationNotFound(situationId))
            : Result.Success<SituationResult?>(ToSituationResult(situation));
    }

    public async Task<Result<Guid?>> AddAsync(Guid groupId, CreateSituationRequest request, CancellationToken cancellationToken)
    {
        await createSituationRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var groupParticipant = await GetCurrentParticipantAsync(groupId, cancellationToken);
        if (groupParticipant is null)
        {
            return Result.Failure<Guid?>(GroupNotFound(groupId));
        }

        var situation = Situation.Create(groupParticipant.Id, request.Text);
        var id = await situationRepository.AddAsync(situation, cancellationToken);

        return Result.Success(id);
    }

    public async Task<Result> UpdateAsync(Guid situationId, Guid groupId, UpdateSituationRequest request,
        CancellationToken cancellationToken)
    {
        await updateSituationRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

        var groupParticipant = await GetCurrentParticipantAsync(groupId, cancellationToken);
        if (groupParticipant is null)
        {
            return Result.Failure(GroupNotFound(groupId));
        }

        var situation = await situationRepository.GetByIdAsync(groupId, situationId, cancellationToken);
        if (situation is null)
        {
            return Result.Failure(SituationNotFound(situationId));
        }

        situation.UpdateText(groupParticipant.Id, request.Text);

        await situationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid groupId, Guid situationId, CancellationToken cancellationToken)
    {
        if (await GetCurrentParticipantAsync(groupId, cancellationToken) is null)
        {
            return Result.Failure(GroupNotFound(groupId));
        }

        var situation = await situationRepository.GetByIdAsync(groupId, situationId, cancellationToken);
        if (situation is null)
        {
            return Result.Failure(SituationNotFound(situationId));
        }

        await situationRepository.DeleteAsync(situation, cancellationToken);
        return Result.Success();
    }

    /// <summary>
    /// The current user's participant in the group, or null when they are not a member (or the group does not exist;
    /// both are reported as "not found" so the API does not reveal other groups).
    /// </summary>
    private Task<GroupParticipant?> GetCurrentParticipantAsync(Guid groupId, CancellationToken cancellationToken)
        => groupParticipantRepository.GetByUserIdAndGroupIdAsync(currentUser.UserId, groupId, cancellationToken);

    private static Error GroupNotFound(Guid groupId) => Error.NotFound($"No group with ID: { groupId } found");

    private static Error SituationNotFound(Guid situationId) => Error.NotFound($"No situation with ID: { situationId } found");

    private static SituationResult ToSituationResult(Situation situation) => new(
        situation.Id,
        situation.Text,
        situation.CreatedById,
        situation.LastModifiedById,
        situation.CreationDate,
        situation.LastModificationDate);
}
