using SharboAPI.Application.DTO.Meme;
using SharboAPI.Application.Abstractions.Services;

namespace SharboAPI.Tests.Fakes;

public class MemeServiceFake(BehaviorFake behavior) : IMemeService
{
    private readonly BehaviorFake _behavior = behavior;

    public Task<Result<IEnumerable<MemeResult>>> GetAllForGroupAsync(Guid groupId, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Success<IEnumerable<MemeResult>>([]));
        }

        MemeResult[] results = [
            new(Guid.NewGuid(), "fake_path", "fake_text", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow),
            new(Guid.NewGuid(), "fake_path", "fake_text", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow),
            new(Guid.NewGuid(), "fake_path", "fake_text", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow),
        ];

        return Task.FromResult(
            Result.Success<IEnumerable<MemeResult>>(results));
    }

    public Task<Result<MemeResult?>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure<MemeResult?>(Error.NotFound("no fake entity found")));
        }

        MemeResult result = new(Guid.NewGuid(), "fake_path", "fake_text", Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);

        return Task.FromResult(
            Result.Success<MemeResult?>(result));
    }

    public Task<Result<Guid?>> AddAsync(CreateMemeRequest request, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure<Guid?>(Error.NotFound("no fake entity found")));
        }

        var createdMemeId = Guid.NewGuid();

        return Task.FromResult(
            Result.Success<Guid?>(createdMemeId));
    }

    public Task<Result> UpdateAsync(Guid id, Guid groupId, UpdateMemeRequest request, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure(Error.Forbidden("you're not allowed to perform this operation")));
        }

        return Task.FromResult(Result.Success());
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!_behavior.IsSuccess)
        {
            return Task.FromResult(
                Result.Failure(Error.Forbidden("you're not allowed to perform this operation")));
        }

        return Task.FromResult(Result.Success());
    }
}
