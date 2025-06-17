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

    public void Update(Guid modifiedById, string text)
    {
        base.Update(modifiedById);
        Text = text;
    }
    #endregion
}
