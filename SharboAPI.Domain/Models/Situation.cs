namespace SharboAPI.Domain.Models;

public class Situation : Entry
{
    public string Text { get; private set; }

    private Situation() {}

    #region Factory_Methods
    public static Situation Create(Guid createdById, string text)
    {
        Situation situation = new()
        {
            Text = text
        };

        Entry.Set(situation, createdById);
        return situation;
    }

    public static Situation Create(GroupParticipant createdBy, string text)
    {
        Situation situation = new()
        {
            Text = text
        };

        Entry.Set(situation, createdBy);
        return situation;
    }

    public static void Update(Situation entity, Guid modifiedById, string text)
    {
        Entry.Update(entity, modifiedById);
        entity.Text = text;
    }

    public static void Update(Situation entity, GroupParticipant modifiedBy, string text)
    {
        Entry.Update(entity, modifiedBy);
        entity.Text = text;
    }
    #endregion
}
