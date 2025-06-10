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
    public List<GroupParticipant> GroupParticipants { get; private set; } = [];
    public DateTime CreationDate { get; private set; }
    public DateTime ModificationDate { get; private set; }

	private Group() {}

	// Factory methods
	public static Group Create(string name,
                               User createdBy,
                               string? imagePath = null,
                               List<GroupParticipant>? participants = null)
		=> new()
		{
			Id = Guid.NewGuid(),
			Name = name,
			ImagePath = imagePath,
			CreatedBy = createdBy,
			LastModifiedBy = createdBy,
            GroupParticipants = participants is not null ? participants : [],
			CreationDate = DateTime.UtcNow,
			ModificationDate = DateTime.UtcNow,
		};

    public static Group Create(string name,
                               Guid createdById,
                               string? imagePath = null,
                               List<GroupParticipant>? participants = null)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            ImagePath = imagePath,
            CreatedById = createdById,
            LastModifiedById = createdById,
            GroupParticipants = participants is not null ? participants : [],
            CreationDate = DateTime.UtcNow,
            ModificationDate = DateTime.UtcNow,
        };

    public static void Update(Group entity,
							  string name,
							  User modifiedBy,
                              string? imagePath = null,
                              List<GroupParticipant>? participants = null)
    {
		entity.Name = name;
		entity.ImagePath = imagePath;
		entity.LastModifiedBy = modifiedBy;
		entity.ModificationDate = DateTime.UtcNow;

        if (participants is not null)
        {
			entity.GroupParticipants = participants;
        }
    }

    public static void Update(Group entity,
                              string name,
                              Guid modifiedById,
                              string? imagePath = null,
                              List<GroupParticipant>? participants = null)
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
