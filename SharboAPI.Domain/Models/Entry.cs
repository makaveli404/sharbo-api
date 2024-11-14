namespace SharboAPI.Domain.Models;

public class Entry
{
	public int Id { get; private set; }
	public DateTime CreationDate { get; private set; }
	public DateTime LastModificationDate { get; private set; }
	public Guid CreatedById { get; private set; }
	public User CreatedBy { get; private set; }
    public Guid LastModifiedById { get; private set; }
    public User LastModifiedBy { get; private set; }
	public List<User> Participants { get; private set; }

	private Entry() {}

	// Factory Methods
	public static Entry Create(Guid createdById, List<User> participants)
		=> new()
		{
			CreatedById = createdById,
			LastModifiedById = createdById,
			Participants = participants,
			CreationDate = DateTime.UtcNow, 
			LastModificationDate = DateTime.UtcNow,
		};

	public static void Update(Entry entity,
                              Guid modifiedById,
                              List<User> participants)
	{
		entity.LastModifiedById = modifiedById;
		entity.Participants = participants;
		entity.LastModificationDate = DateTime.UtcNow;
	}
}
