namespace SharboAPI.Domain.Models;

public class Group
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string? ImagePath { get; private set; }
	public Guid CreatedById { get; private set; }
	public User CreatedBy { get; private set; }
	public Guid LastModifiedById { get; private set; }
    public User LastModifiedBy { get; private set; }
    public List<GroupParticipants> GroupParticipants { get; private set; } = [];
    public DateTime CreationDate { get; private set; }
    public DateTime ModificationDate { get; private set; }

	private Group() {}

	// Factory methods
	public static Group Create(string name,
                               Guid createdById,
                               List<GroupParticipants> participants,
                               string? imagePath = null)
		=> new()
		{
			Id = Guid.NewGuid(),
			Name = name,
			ImagePath = imagePath,
			CreatedById = createdById,
			LastModifiedById = createdById,
            GroupParticipants = participants,
			CreationDate = DateTime.UtcNow,
			ModificationDate = DateTime.UtcNow,
		};

    public static void Update(Group entity,
							  string name,
							  Guid modifiedById,
                              string? imagePath = null,
                              List<GroupParticipants>? participants = null)
    {
		entity.Name = name;
		entity.ImagePath = imagePath;
		entity.LastModifiedById = modifiedById;
		entity.ModificationDate = DateTime.UtcNow;

        if (participants is not null)
        {
			entity.GroupParticipants = participants;
        }
    }
}
