using SharboAPI.Application.DTO.GroupParticipant;

namespace SharboAPI.Application.DTO.Group;

public record GroupDto(string Name, string? ImagePath, List<GroupParticipantDto>? Participants = null);
