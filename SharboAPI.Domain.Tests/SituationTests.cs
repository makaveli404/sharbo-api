namespace SharboAPI.Domain.Tests;

public class SituationTests
{
	[Fact]
	public void Create_ShouldInitializeSituationCorrectly()
	{
		// Arrange
		var createdById = Guid.NewGuid();
		const string text = "This is a test situation.";

		var expectedSituation = new
		{
			Text = text
		};

		// Act
		var situation = Situation.Create(createdById, text);

		// Assert
		situation.Should().BeEquivalentTo(expectedSituation);
	}

	[Fact]
	public void Update_ShouldUpdateSituationCorrectly()
	{
		// Arrange
		var createdById = Guid.NewGuid();
		var situation = Situation.Create(createdById, "Initial situation text.");

		var modifiedById = Guid.NewGuid();
		const string updatedText = "This is the updated situation text.";

		var expectedSituation = new
		{
			Text = updatedText
		};

		// Act
		situation.Update(modifiedById, updatedText);

		// Assert
		situation.Should().BeEquivalentTo(expectedSituation);
	}

	[Fact]
	public void Update_ShouldThrowException_WhenEntityIsNull()
	{
		// Arrange
		Situation? situation = null;
		var modifiedById = Guid.NewGuid();
		const string text = "Updated situation text.";

		// Act
		var act = () => situation.Update(modifiedById, text);

		// Assert
		act.Should().Throw<NullReferenceException>();
	}
}
