using System.ComponentModel.DataAnnotations;

namespace SharboAPI.Domain.Models;

public class Entry
{
	[Key]
	public int Id { get; set; }
	public DateTime CreationDate { get; set; }
	public DateTime LastModificationDate { get; set; }
	public User LastModifiedBy { get; set; }
	public List<User> Participants { get; set; }
}
