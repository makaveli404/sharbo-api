namespace SharboAPI.Domain.Models;

public class GroupParticipant
{
	public Guid Id { get; private set; }
	public Guid GroupId { get; private set; }
	public Group Group { get; private set; }
	public string UserId { get; private set; }
	public User User { get; private set; }

	public List<GroupParticipantRole> GroupParticipantRoles { get; private set; } = [];
	public List<Entry> CreatedEntries { get; private set; }
	public List<Entry> ModifiedEntries { get; private set; }

	private GroupParticipant() {}

	#region Factory_Methods
	public static GroupParticipant Create(string userId, List<GroupParticipantRole>? participantRoles = null)
		=> new()
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			GroupParticipantRoles = participantRoles is not null ? participantRoles : []
		};

	public static GroupParticipant Create(Guid groupId,
		string userId,
		List<GroupParticipantRole>? participantRoles = null)
	{
		GroupParticipant groupParticipant = Create(userId, participantRoles);
		groupParticipant.GroupId = groupId;

		return groupParticipant;
	}

	public void AddRole(GroupParticipantRole role) => GroupParticipantRoles.Add(role);
	public void RemoveRole(GroupParticipantRole role) => GroupParticipantRoles.Remove(role);

	#endregion
}
