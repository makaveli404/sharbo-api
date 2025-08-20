namespace SharboAPI.Domain.Models;

public class Quote : Entry
{
	public string Text { get; private set; }

	private Quote() {}

    #region Factory_Methods
    public static Quote Create(GroupParticipant createdBy, string text)
    {
        Quote quote = new()
        {
            Text = text
        };

        Entry.Set(quote, createdBy);
        return quote;
    }

    public void Update(GroupParticipant modifiedBy, string text)
    {
        base.Update(modifiedBy);
        Text = text;
    }
    #endregion
}
