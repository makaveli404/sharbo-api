namespace SharboAPI.Domain.Models;

public class Situation
{
	public int EntryId { get; private set; }
    public Entry Entry { get; private set; }
    public string Text { get; private set; }

    private Situation() {}

    // Factory methods
    public static Situation Create(string createdByEmail,
                                   List<User> participants,
                                   string text)
        => new()
        {
            Entry = Entry.Create(createdByEmail, participants),
            Text = text
        };

    public static void Update(Situation entity,
                              string modifiedByEmail,
                              List<User> participants,
                              string text)
    {
        Entry.Update(entity.Entry, modifiedByEmail, participants);
        entity.Text = text;
    }
}
