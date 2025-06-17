namespace SharboAPI.Domain.Models;

public class Meme : Entry
{
	public string ImagePath { get; private set; }
	public string? Text { get; private set; }

	private Meme() {}

    #region Factory_Methods
    public static Meme Create(Guid createdById, string imagePath, string? text = null)
    {
        Meme meme = new()
        {
            ImagePath = imagePath,
            Text = text
        };

        Entry.Set(meme, createdById);
        return meme;
    }

    public void Update(Guid modifiedById, string imagePath, string? text = null)
    {
        base.Update(modifiedById);
        ImagePath = imagePath;
        Text = text;
    }
    #endregion
}
