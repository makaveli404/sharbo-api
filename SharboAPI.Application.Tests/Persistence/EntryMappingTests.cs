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
	[InlineData(typeof(Meme), nameof(Meme))]
	[InlineData(typeof(Quote), nameof(Quote))]
	[InlineData(typeof(Situation), nameof(Situation))]
	public void EntryTypes_AreMappedToSharedEntriesTableWithTypeDiscriminator(Type entryType, string expectedDiscriminator)
	{
		// Arrange
		using var context = CreateContext();

		// Act
		var entity = context.Model.FindEntityType(entryType)!;

		// Assert
		entity.GetTableName().Should().Be("Entries");
		entity.FindDiscriminatorProperty()!.Name.Should().Be("Type");
		entity.GetDiscriminatorValue().Should().Be(expectedDiscriminator);
	}

	[Fact]
	public void CreateScript_ContainsSingleEntriesTableWithOneTextColumn()
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
		script.Should().Contain("\"Text\" text,");
		script.Should().NotContainAny("\"Quote_Text\"", "\"Situation_Text\"", "\"Meme_Text\"");
	}

	[Fact]
	public void Model_HasNoChangesMissingFromMigrations()
	{
		// Arrange
		using var context = CreateContext();

		// Act
		var hasPendingChanges = context.Database.HasPendingModelChanges();

		// Assert
		hasPendingChanges.Should().BeFalse("every model change needs a migration (dotnet ef migrations add)");
	}
}
