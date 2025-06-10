namespace SharboAPI.Domain.Models;

public class GroupParticipantRole
{
    public Guid GroupParticipantId { get; private set; }
    public GroupParticipant GroupParticipant { get; private set; }
    public int RoleId { get; private set; }
    public Role Role { get; private set; }

    private GroupParticipantRole() {}

    #region Factory_Methods
    public static GroupParticipantRole Create(int roleId) 
        => new() 
        { 
            RoleId = roleId 
        };

    public static GroupParticipantRole Create(Role role)
        => new()
        {
            Role = role
        };

    public static GroupParticipantRole Create(Guid groupParticipantId, int roleId)
        => new()
        {
            GroupParticipantId = groupParticipantId,
            RoleId = roleId
        };

    public static GroupParticipantRole Create(GroupParticipant groupParticipant, int roleId)
        => new()
        {
            GroupParticipant = groupParticipant,
            RoleId = roleId
        };

    public static GroupParticipantRole Create(Guid groupParticipantId, Role role)
        => new()
        {
            GroupParticipantId = groupParticipantId,
            Role = role
        };

    public static GroupParticipantRole Create(GroupParticipant groupParticipant, Role role)
        => new()
        {
            GroupParticipant = groupParticipant,
            Role = role
        };

    public static void Update(GroupParticipantRole entity, int roleId) => entity.RoleId = roleId;
    public static void Update(GroupParticipantRole entity, Role role) => entity.Role = role;
    #endregion
}
