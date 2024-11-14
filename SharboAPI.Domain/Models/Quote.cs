namespace SharboAPI.Domain.Models;

public class Quote
{
	public int EntryId { get; private set; }
	public Entry Entry { get; private set; }
	public string Text { get; private set; }

	private Quote() {}

    // Factory methods
    public static Quote Create(Guid createdById,
                               List<User> participants,
                               string text)
    => new()
    {
        Entry = Entry.Create(createdById, participants),
        Text = text
    };

    public static void Update(Quote entity,
                              Guid modifiedById,
                              List<User> participants,
                              string text)
    {
        Entry.Update(entity.Entry, modifiedById, participants);
        entity.Text = text;
    }
}
