namespace SharboAPI.Domain.Models;

public class Situation
{
	public int EntryId { get; private set; }
    public Entry Entry { get; private set; }
    public string Text { get; private set; }

    private Situation() {}

    // Factory methods
    public static Situation Create(Guid createdById,
                                   List<User> participants,
                                   string text)
        => new()
        {
            Entry = Entry.Create(createdById, participants),
            Text = text
        };

    public static void Update(Situation entity,
                              Guid modifiedById,
                              List<User> participants,
                              string text)
    {
        Entry.Update(entity.Entry, modifiedById, participants);
        entity.Text = text;
    }
}
