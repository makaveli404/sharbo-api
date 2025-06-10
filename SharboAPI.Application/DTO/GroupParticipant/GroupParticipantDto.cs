namespace SharboAPI.Application.DTO.GroupParticipant;

public record GroupParticipantDto(Guid GroupId, Guid UserId, List<string> Roles);
