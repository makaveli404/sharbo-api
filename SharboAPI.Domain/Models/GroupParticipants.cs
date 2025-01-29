namespace SharboAPI.Domain.Models;

public class GroupParticipants
{
    public Guid GroupId { get; private set; }
    public Group Group { get; private set; }
    public string UserEmail { get; private set; }
    public User User { get; private set; }
    public bool IsAdmin { get; private set; }

    private GroupParticipants() {}

    // Factory methods
    public static GroupParticipants Create(Guid groupId, string userEmail, bool isAdmin)
        => new()
        {
            GroupId = groupId,
            UserEmail = userEmail,
            IsAdmin = isAdmin
        };

    public static void UpdateAdminStatus(GroupParticipants entity, bool isAdmin)
        => entity.IsAdmin = isAdmin;
}
