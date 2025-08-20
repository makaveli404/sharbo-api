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
    protected static void Set(Entry entry, GroupParticipant createdBy)
	{
        entry.Id = Guid.NewGuid();
        entry.CreatedBy = createdBy;
        entry.CreationDate = DateTime.UtcNow;

        entry.Update(createdBy);
	}

    protected void Update(GroupParticipant modifiedBy)
	{
		LastModifiedBy = modifiedBy;
		LastModificationDate = DateTime.UtcNow;
	}

    #endregion
}
