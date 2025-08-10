using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.DTO.GroupParticipant;

public record GroupParticipantResult(Guid Id, Guid UserId, ICollection<string> Roles);
