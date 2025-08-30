namespace SharboAPI.Domain.Models;

public class Group
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string? ImagePath { get; private set; }
	public string CreatedById { get; private set; }
	public User CreatedBy { get; private set; }
	public string LastModifiedById { get; private set; }
	public User LastModifiedBy { get; private set; }
	public List<GroupParticipant> GroupParticipants { get; private set; } = [];
	public DateTime CreationDate { get; private set; }
	public DateTime ModificationDate { get; private set; }

	private Group() {}

	#region Factory_Methods
	public static Group Create(string name,
		string createdById,
		string? imagePath = null,
		List<GroupParticipant>? participants = null)
	{
		Group group = new()
		{
			Id = Guid.NewGuid(),
			CreatedById = createdById,
			CreationDate = DateTime.UtcNow
		};

		group.Update(name, createdById, imagePath);
		group.AddParticipants(participants);

		return group;
	}

	public void Update(string name,
		string modifiedById,
		string? imagePath = null)
	{
		Name = name;
		ImagePath = imagePath;
		LastModifiedById = modifiedById;
		ModificationDate = DateTime.UtcNow;
	}

	public void AddParticipants(List<GroupParticipant>? participants)
	{
		if (participants != null)
		{
			GroupParticipants.AddRange(participants);
		}
	}

	public void RemoveParticipants(List<GroupParticipant>? participants)
	{
		if (participants != null)
		{
			GroupParticipants.RemoveAll(participants.Contains);
		}
	}
	#endregion
}
