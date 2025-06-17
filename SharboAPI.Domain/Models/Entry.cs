namespace SharboAPI.Domain.Models;

public class Entry
{
	public Guid Id { get; protected set; }
	public DateTime CreationDate { get; protected set; }
	public DateTime LastModificationDate { get; protected set; }
	public Guid CreatedById { get; protected set; }
	public GroupParticipant CreatedBy { get; protected set; }
    public Guid LastModifiedById { get; protected set; }
    public GroupParticipant LastModifiedBy { get; protected set; }

    protected Entry() {}

    #region Factory_Methods
    protected static void Set(Entry entry, Guid createdById)
	{
        entry.Id = Guid.NewGuid();
        entry.CreatedById = createdById;
        entry.CreationDate = DateTime.UtcNow;

        entry.Update(createdById);
	}

    protected static void Set(Entry entry, GroupParticipant createdBy)
        => Set(entry, createdBy.Id);

    protected void Update(Guid modifiedById)
	{
		LastModifiedById = modifiedById;
		LastModificationDate = DateTime.UtcNow;
	}

    protected void Update(GroupParticipant modifiedBy)
        => Update(modifiedBy.Id);
    #endregion
}
