using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Email);
        builder.Property(u => u.Email).IsRequired();

        builder
            .HasMany(u => u.Entries)
            .WithMany(e => e.Participants)
            .UsingEntity(e => e.ToTable("EntryParticipants"));
    }
}
