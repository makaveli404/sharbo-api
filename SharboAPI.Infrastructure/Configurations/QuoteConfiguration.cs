using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
	public void Configure(EntityTypeBuilder<Quote> builder)
	{
		// Shares the Entries.Text column with the other entry types (TPH).
		builder
			.Property(e => e.Text)
			.HasColumnName(EntryConfiguration.TextColumnName);
	}
}
