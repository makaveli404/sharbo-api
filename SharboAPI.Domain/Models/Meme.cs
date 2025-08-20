namespace SharboAPI.Domain.Models;

public class Meme : Entry
{
	public string ImagePath { get; private set; }
	public string? Text { get; private set; }

	private Meme() {}

    #region Factory_Methods
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

    public void UpdateText(GroupParticipant modifiedBy, string text)
    {
        base.Update(modifiedBy);
        Text = text;
    }
    #endregion
}
