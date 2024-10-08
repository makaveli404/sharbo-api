namespace SharboAPI.Domain.Models;

public class Event
{
	public int Id { get; set; }
	public string Text { get; set; }
	public string Image { get; set; }
	public DateTime CreationDate { get; set; }
	public User CreatedBy { get; set; }
	public int EventTypeId { get; set; }
}
