using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class SituationConfiguration : IEntityTypeConfiguration<Situation>
{
	public void Configure(EntityTypeBuilder<Situation> builder)
	{
		builder.ToTable("Situations");
	}
}
