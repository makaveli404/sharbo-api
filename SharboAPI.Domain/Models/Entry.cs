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
        entry.LastModifiedById = createdById;
        entry.CreationDate = DateTime.UtcNow;
        entry.LastModificationDate = DateTime.UtcNow;
	}

    protected static void Set(Entry entry, GroupParticipant createdBy)
    {
        entry.Id = Guid.NewGuid();
        entry.CreatedBy = createdBy;
        entry.LastModifiedBy = createdBy;
        entry.CreationDate = DateTime.UtcNow;
        entry.LastModificationDate = DateTime.UtcNow;
    }

    protected static void Update(Entry entity, Guid modifiedById)
	{
		entity.LastModifiedById = modifiedById;
		entity.LastModificationDate = DateTime.UtcNow;
	}

    protected static void Update(Entry entity, GroupParticipant modifiedBy)
    {
        entity.LastModifiedBy = modifiedBy;
        entity.LastModificationDate = DateTime.UtcNow;
    }
    #endregion
}
