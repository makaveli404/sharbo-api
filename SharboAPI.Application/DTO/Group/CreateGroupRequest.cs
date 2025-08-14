namespace SharboAPI.Application.DTO.Group;

public sealed record CreateGroupRequest(string Name, string? ImagePath, List<Guid>? Participants = null);
