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

	public void Update(Guid modifiedById, string text)
	{
		base.Update(modifiedById);
		Text = text;
	}
	#endregion
}
