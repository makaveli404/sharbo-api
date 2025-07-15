namespace SharboAPI.Domain.Tests;

public class GroupTests
{
    [Fact]
    public void Create_ShouldInitializeGroupCorrectly()
    {
        // Arrange
        const string NAME = "Test Group";
        var createdById = Guid.NewGuid();
        var participants = TestDataFactory.CreateGroupParticipants(2);
        const string imagePath = "test_image.jpg";

        var expectedGroup = new
        {
            Name = NAME,
            ImagePath = imagePath,
            CreatedById = createdById,
            LastModifiedById = createdById,
            GroupParticipants = participants
        };

        // Act
        var group = Group.Create(NAME, createdById, imagePath, participants);

        // Assert
        group.Should().NotBeNull();
        group.Should().BeEquivalentTo(expectedGroup, options => options
            .ExcludingMissingMembers()
            .Excluding(g => g.GroupParticipants));
        group.GroupParticipants.Should().HaveCount(2);
        group.GroupParticipants.Should().BeEquivalentTo(participants);
        group.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldUpdateGroupCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = TestDataFactory.CreateGroupParticipants(1);
        var group = Group.Create("Initial Group", createdById, "initial_image.jpg", initialParticipants);

        const string UPDATED_NAME = "Updated Group";
        var modifiedById = Guid.NewGuid();
        const string UPDATED_IMAGE_PATH = "updated_image.jpg";

        var expectedGroup = new
        {
            Name = UPDATED_NAME,
            ImagePath = UPDATED_IMAGE_PATH,
            CreatedById = createdById,
            LastModifiedById = modifiedById,
            GroupParticipants = initialParticipants
        };

        // Act
        group.Update(UPDATED_NAME, modifiedById, UPDATED_IMAGE_PATH);

        // Assert
        group.Should().NotBeNull();
        group.Should().BeEquivalentTo(expectedGroup, options => options
            .ExcludingMissingMembers()
            .Excluding(g => g.GroupParticipants));
        group.GroupParticipants.Should().HaveCount(1);
        group.GroupParticipants.Should().BeEquivalentTo(initialParticipants);
        group.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldNotChangeParticipants_WhenParticipantsAreNull()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = TestDataFactory.CreateGroupParticipants(1);
        var group = Group.Create("Initial Group", createdById, participants: initialParticipants);

        const string UPDATED_NAME = "Updated Group";
        var modifiedById = Guid.NewGuid();
        const string UPDATED_IMAGE_PATH = "updated_image.jpg";

        var expectedGroup = new
        {
            Name = UPDATED_NAME,
            ImagePath = UPDATED_IMAGE_PATH,
            CreatedById = createdById,
            LastModifiedById = modifiedById,
            GroupParticipants = initialParticipants
        };

        // Act
        group.Update(UPDATED_NAME, modifiedById, UPDATED_IMAGE_PATH);

        // Assert
        group.Should().NotBeNull();
        group.Should().BeEquivalentTo(expectedGroup, options => options
            .ExcludingMissingMembers()
            .Excluding(g => g.GroupParticipants));
        group.GroupParticipants.Should().HaveCount(1);
        group.GroupParticipants.Should().BeEquivalentTo(initialParticipants);
        group.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Group group = null;
        const string UPDATED_NAME = "Updated Group";
        var modifiedById = Guid.NewGuid();

        // Act
        var act = () => group.Update(UPDATED_NAME, modifiedById);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
