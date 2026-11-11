using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
	public const string TextColumnName = "Text";
	public const string TypeColumnName = "Type";

	public void Configure(EntityTypeBuilder<Entry> builder)
	{
		builder.ToTable("Entries");

		// Table-per-Hierarchy: Meme, Quote and Situation share the Entries table, Type is the discriminator.
		builder
			.HasDiscriminator<string>(TypeColumnName);

		builder.Property<string>(TypeColumnName).HasMaxLength(20);

		builder
			.HasOne(e => e.CreatedBy)
			.WithMany(gp => gp.CreatedEntries)
			.HasForeignKey(e => e.CreatedById);

		builder
			.HasOne(e => e.LastModifiedBy)
			.WithMany(gp => gp.ModifiedEntries)
			.HasForeignKey(e => e.LastModifiedById);
	}
}
