namespace SharboAPI.Application.DTO.GroupParticipant;

public sealed record GroupParticipantResult(Guid Id, string UserId, ICollection<string> Roles);
