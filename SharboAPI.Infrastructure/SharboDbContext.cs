using Microsoft.EntityFrameworkCore;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure;

public class SharboDbContext(DbContextOptions<SharboDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<Role> Roles { get; set; }
	public DbSet<Event> Events { get; set; }
	public DbSet<EventType> EventTypes { get; set; }
	public DbSet<Group> Groups { get; set; }
}
