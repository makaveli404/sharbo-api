using Microsoft.EntityFrameworkCore;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure;

public class SharboDbContext(DbContextOptions<SharboDbContext> options) : DbContext(options)
{
	public DbSet<User> Users { get; set; }
	public DbSet<Entry> Entries { get; set; }
	public DbSet<Meme> Memes { get; set; }
	public DbSet<Quote> Quotes { get; set; }
	public DbSet<Situation> Situations { get; set; }
	public DbSet<Group> Groups { get; set; }
	public DbSet<GroupParticipant> GroupParticipants { get; set; }
	public DbSet<Role> Roles { get; set; }
	public DbSet<GroupParticipantRole> GroupParticipantRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharboDbContext).Assembly);
    }
}
