using System.ComponentModel.DataAnnotations;

namespace SharboAPI.Domain.Models;

public class Meme
{
	[Key]
	public int EntryId { get; set; }
	public string Image { get; set; }
	public string Text { get; set; }
}
