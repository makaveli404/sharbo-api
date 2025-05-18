namespace SharboAPI.Domain.Models;

public class Group
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string? ImagePath { get; private set; }
	public string CreatedByEmail { get; private set; }
	public User CreatedBy { get; private set; }
	public string LastModifiedByEmail { get; private set; }
    public User LastModifiedBy { get; private set; }
    public List<GroupParticipants> GroupParticipants { get; private set; } = [];
    public DateTime CreationDate { get; private set; }
    public DateTime ModificationDate { get; private set; }

	private Group() {}

	// Factory methods
	public static Group Create(string name,
                               string createdByEmail,
                               List<GroupParticipants> participants,
                               string? imagePath = null)
		=> new()
		{
			Id = Guid.NewGuid(),
			Name = name,
			ImagePath = imagePath,
			CreatedByEmail = createdByEmail,
			LastModifiedByEmail = createdByEmail,
            GroupParticipants = participants,
			CreationDate = DateTime.UtcNow,
			ModificationDate = DateTime.UtcNow,
		};

    public static void Update(Group entity,
							  string name,
							  string modifiedByEmail,
                              string? imagePath = null,
                              List<GroupParticipants>? participants = null)
    {
		entity.Name = name;
		entity.ImagePath = imagePath;
		entity.LastModifiedByEmail = modifiedByEmail;
		entity.ModificationDate = DateTime.UtcNow;

        if (participants is not null)
        {
			entity.GroupParticipants = participants;
        }
    }
}
