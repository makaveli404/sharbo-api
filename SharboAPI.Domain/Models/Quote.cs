namespace SharboAPI.Domain.Models;

public class Quote
{
	public int EntryId { get; private set; }
	public Entry Entry { get; private set; }
	public string Text { get; private set; }

	private Quote() {}

    // Factory methods
    public static Quote Create(string createdByEmail,
                               List<User> participants,
                               string text)
    => new()
    {
        Entry = Entry.Create(createdByEmail, participants),
        Text = text
    };

    public static void Update(Quote entity,
                              string modifiedByEmail,
                              List<User> participants,
                              string text)
    {
        Entry.Update(entity.Entry, modifiedByEmail, participants);
        entity.Text = text;
    }
}
