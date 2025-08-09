namespace SharboAPI.Application.DTO.Group;

public record CreateGroup(string Name, string? ImagePath, List<Guid>? Participants = null);
