namespace SharboAPI.Application.DTO.User;

public record UserDetailsDto(Guid Id, string FirebaseUserId, string Email, string Nickname);
