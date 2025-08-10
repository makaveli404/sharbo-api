using SharboAPI.Domain.Enums;

namespace SharboAPI.Application.DTO.GroupParticipant;

public record UpdateGroupParticipantRoles(List<RoleType> RoleTypes);
