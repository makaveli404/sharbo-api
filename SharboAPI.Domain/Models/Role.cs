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
	{
		Role role = new();
		role.Update(roleType, name);

		return role;

	}

	public void Update(RoleType roleType, string name)
	{
		RoleType = roleType;
		Name = name;
	}
	#endregion
}
