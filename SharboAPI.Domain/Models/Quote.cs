using System.ComponentModel.DataAnnotations;

namespace SharboAPI.Domain.Models;

public class Quote
{
	[Key]
	public int EntryId { get; set; }
	public string? Text { get; set; }
}
