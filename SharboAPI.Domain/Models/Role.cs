using SharboAPI.Domain.Enums;

namespace SharboAPI.Domain.Models;

public class Role
{
    public int Id { get; private set; }
    public RoleType RoleType { get; private set; }
    public string Name { get; private set; }
    public List<GroupParticipantRole> GroupParticipantRoles { get; private set; } = [];

    private Role() {}

    #region Factory_Methods
    public static Role Create(RoleType roleType, string name)
        => new()
        {
            RoleType = roleType,
            Name = name
        };

    public static void Update(Role entity, RoleType roleType, string name)
    {
        entity.RoleType = roleType;
        entity.Name = name;
    }
    #endregion
}
