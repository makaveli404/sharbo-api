using SharboAPI.Domain.Enums;

namespace SharboAPI.Application.DTO.GroupParticipant;

public sealed record UpdateGroupParticipantRolesRequest(List<string> Roles);
