using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
	public const string TextColumnName = "Text";

	public void Configure(EntityTypeBuilder<Entry> builder)
	{
		var allowedTypes = string.Join(", ", Enum.GetNames<EntryType>().Select(name => $"'{name}'"));

		builder.ToTable("Entries", table =>
			table.HasCheckConstraint("CK_Entries_Type", $"\"Type\" IN ({allowedTypes})"));

		// Table-per-Hierarchy: Meme, Quote and Situation share the Entries table, Type is the discriminator.
		builder
			.HasDiscriminator(e => e.Type)
			.HasValue<Meme>(EntryType.Meme)
			.HasValue<Quote>(EntryType.Quote)
			.HasValue<Situation>(EntryType.Situation);

		builder
			.Property(e => e.Type)
			.HasConversion<string>()
			.HasMaxLength(20);

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
