namespace SharboAPI.Domain.Models;

public class Meme
{
	public int EntryId { get; private set; }
	public Entry Entry { get; private set; }
	public string ImagePath { get; private set; }
	public string? Text { get; private set; }

	private Meme() {}

    // Factory methods
    public static Meme Create(string createdByEmail,
                              List<User> participants,
                              string imagePath,
                              string? text = null)
        => new()
        {
            Entry = Entry.Create(createdByEmail, participants),
            ImagePath = imagePath,
            Text = text
        };

    public static void Update(Meme entity,
                              string modifiedByEmail,
                              List<User> participants,
                              string imagePath,
                              string? text = null)
    {
        Entry.Update(entity.Entry, modifiedByEmail, participants);
        entity.ImagePath = imagePath;
        entity.Text = text;
    }
}
