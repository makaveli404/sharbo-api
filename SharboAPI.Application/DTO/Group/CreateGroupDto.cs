using SharboAPI.Application.DTO.GroupParticipant;

namespace SharboAPI.Application.DTO.Group;

public record CreateGroupDto(string Name, string? ImagePath, List<GroupParticipantDto>? Participants = null);
