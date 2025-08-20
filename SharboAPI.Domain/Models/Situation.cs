namespace SharboAPI.Domain.Models;

public class Situation : Entry
{
    public string Text { get; private set; }

    private Situation() {}

    #region Factory_Methods
    public static Situation Create(GroupParticipant createdBy, string text)
    {
        Situation situation = new()
        {
            Text = text
        };

        Entry.Set(situation, createdBy);
        return situation;
    }

    public void Update(GroupParticipant modifiedBy, string text)
    {
        base.Update(modifiedBy);
        Text = text;
    }
    #endregion
}
