namespace SharboAPI.Domain.Models;

public class Entry
{
	public int Id { get; private set; }
	public DateTime CreationDate { get; private set; }
	public DateTime LastModificationDate { get; private set; }
	public string CreatedByEmail { get; private set; }
	public User CreatedBy { get; private set; }
    public string LastModifiedByEmail { get; private set; }
    public User LastModifiedBy { get; private set; }
	public List<User> Participants { get; private set; }

	private Entry() {}

	// Factory Methods
	public static Entry Create(string createdByEmail, List<User> participants)
		=> new()
		{
			CreatedByEmail = createdByEmail,
			LastModifiedByEmail = createdByEmail,
			Participants = participants,
			CreationDate = DateTime.UtcNow, 
			LastModificationDate = DateTime.UtcNow,
		};

	public static void Update(Entry entity,
                              string modifiedByEmail,
                              List<User> participants)
	{
		entity.LastModifiedByEmail = modifiedByEmail;
		entity.Participants = participants;
		entity.LastModificationDate = DateTime.UtcNow;
	}
}
