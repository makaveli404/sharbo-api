using SharboAPI.Application.DTO.GroupParticipant;

namespace SharboAPI.Application.DTO.Group;

public sealed record GroupResult(Guid Id, string Name, string? ImagePath, Guid CreatedById, ICollection<GroupParticipantResult> Participants);
