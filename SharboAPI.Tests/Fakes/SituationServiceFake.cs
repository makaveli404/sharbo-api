using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Situation;

namespace SharboAPI.Tests.Fakes;

public class SituationServiceFake(BehaviorFake behavior) : ISituationService
{
    private readonly BehaviorFake _behavior = behavior;

    public Task<Result<IEnumerable<SituationResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Success<IEnumerable<SituationResult>>([]));
        }

        SituationResult[] situationResults = [
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now),
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now),
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now)
        ];

        return Task.FromResult(
            Result.Success<IEnumerable<SituationResult>>(situationResults));
    }

    public Task<Result<SituationResult?>> GetByIdAsync(Guid situationId, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure<SituationResult?>(Error.NotFound("no situation found")));
        }

        SituationResult situationResult =
            new(Guid.NewGuid(), "text", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, DateTime.Now);

        return Task.FromResult(
            Result.Success<SituationResult?>(situationResult));
    }

    public Task<Result<Guid?>> AddAsync(CreateSituationRequest request, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure<Guid?>(Error.NotFound("no fake entity found")));
        }

        Guid createdSituationId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");

        return Task.FromResult(
            Result.Success<Guid?>(createdSituationId));
    }

    public Task<Result> UpdateAsync(Guid situationId, Guid groupId, UpdateSituationRequest request, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(Result.Failure(Error.Forbidden("you're not allowed to perform this operation")));
        }

        return Task.FromResult(Result.Success());
    }

    public Task<Result> DeleteAsync(Guid situationId, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(Result.Failure(Error.Forbidden("you're not allowed to perform this operation")));
        }

        return Task.FromResult(Result.Success());
    }
}
