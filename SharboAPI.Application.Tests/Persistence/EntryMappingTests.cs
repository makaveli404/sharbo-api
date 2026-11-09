using Microsoft.EntityFrameworkCore;
using SharboAPI.Infrastructure;

namespace SharboAPI.Application.Tests.Persistence;

public class EntryMappingTests
{
	private static SharboDbContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<SharboDbContext>()
			.UseNpgsql("Host=localhost;Database=sharbo_mapping_tests")
			.Options;

		return new SharboDbContext(options);
	}

	[Theory]
	[InlineData(typeof(Meme))]
	[InlineData(typeof(Quote))]
	[InlineData(typeof(Situation))]
	public void EntryTypes_AreMappedToSharedEntriesTable(Type entryType)
	{
		// Arrange
		using var context = CreateContext();

		// Act
		var entity = context.Model.FindEntityType(entryType)!;

		// Assert
		entity.GetTableName().Should().Be("Entries");
		entity.FindDiscriminatorProperty()!.Name.Should().Be(nameof(Entry.Type));
	}

	[Fact]
	public void CreateScript_ContainsSingleEntriesTableWithOneTextColumnAndTypeCheck()
	{
		// Arrange
		using var context = CreateContext();

		// Act
		var script = context.Database.GenerateCreateScript();

		// Assert
		script.Should().Contain("CREATE TABLE \"Entries\"");
		script.Should().NotContain("CREATE TABLE \"Memes\"");
		script.Should().NotContain("CREATE TABLE \"Quotes\"");
		script.Should().NotContain("CREATE TABLE \"Situations\"");
		script.Should().Contain("CONSTRAINT \"CK_Entries_Type\" CHECK (\"Type\" IN ('Meme', 'Quote', 'Situation'))");
		script.Should().Contain("\"Text\" text,");
		script.Should().NotContainAny("\"Quote_Text\"", "\"Situation_Text\"", "\"Meme_Text\"");
	}
}
