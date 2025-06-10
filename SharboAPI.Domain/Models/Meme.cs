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

    public static Meme Create(GroupParticipant createdBy, string imagePath, string? text = null)
    {
        Meme meme = new()
        {
            ImagePath = imagePath,
            Text = text
        };

        Entry.Set(meme, createdBy);
        return meme;
    }

    public static void Update(Meme entity,
                              Guid modifiedById,
                              string imagePath,
                              string? text = null)
    {
        Entry.Update(entity, modifiedById);
        entity.ImagePath = imagePath;
        entity.Text = text;
    }

    public static void Update(Meme entity,
                              GroupParticipant modifiedBy,
                              string imagePath,
                              string? text = null)
    {
        Entry.Update(entity, modifiedBy);
        entity.ImagePath = imagePath;
        entity.Text = text;
    }
    #endregion
}
