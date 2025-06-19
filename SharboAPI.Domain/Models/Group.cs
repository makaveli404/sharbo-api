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

    #region Factory_Methods
    public static Group Create(string name,
                               Guid createdById,
                               string? imagePath = null,
                               List<GroupParticipant>? participants = null)
    {
        Group group = new()
        {
            Id = Guid.NewGuid(),
            CreatedById = createdById,
            CreationDate = DateTime.UtcNow
        };

        group.Update(name, createdById, imagePath, participants);
        
        return group;
    }

    public void Update(string name,
                       Guid modifiedById,
                       string? imagePath = null,
                       List<GroupParticipant>? participants = null)
    {
        Name = name;
        ImagePath = imagePath;
        LastModifiedById = modifiedById;
        ModificationDate = DateTime.UtcNow;

        if (participants is not null)
        {
            GroupParticipants = participants;
        }
    }
    #endregion
}
