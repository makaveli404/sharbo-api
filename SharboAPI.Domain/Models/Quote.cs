namespace SharboAPI.Domain.Models;

public class Quote : Entry
{
	public string Text { get; private set; }

	private Quote() {}

    #region Factory_Methods
    public static Quote Create(Guid createdById, string text)
    {
        Quote quote = new()
        {
            Text = text
        };

        Entry.Set(quote, createdById);
        return quote;
    }

    public static Quote Create(GroupParticipant createdBy, string text)
    {
        Quote quote = new()
        {
            Text = text
        };

        Entry.Set(quote, createdBy);
        return quote;
    }

    public static void Update(Quote entity, Guid modifiedById, string text)
    {
        Entry.Update(entity, modifiedById);
        entity.Text = text;
    }

    public static void Update(Quote entity, GroupParticipant modifiedBy, string text)
    {
        Entry.Update(entity, modifiedBy);
        entity.Text = text;
    }
    #endregion
}
