using SharboAPI.Domain.Models;
using FluentAssertions;

namespace SharboAPI.Domain.Tests.Models;

public class EntryTest
{
    [Fact]
    public void Create_ForGivenParams_ReturnsNewInstance()
    {
        // Act
        var entry = CreateWithDefaults();
            
        // Assert
        entry
            .Should()
            .BeOfType<Entry>();
    }

    [Fact]
    public void Update_ForGivenParams_UpdatesValuesOfExistingInstance()
    {
        // Arrange
        Guid expectedModifiedById = Guid.NewGuid();
        List<User> expectedParticipants = [
            User.Create("Mythview", "mythview@gmail.com"),
            User.Create("BartTux", "barttux@gmail.com"),
        ];

        var entry = CreateWithDefaults();
        var expectedEntry = Entry.Create(expectedModifiedById, expectedParticipants);

        // Act
        Entry.Update(entry, expectedModifiedById, expectedParticipants);

        // Assert
        entry
            .Should()
            .BeEquivalentTo(expectedEntry, options => options
                .Including(e => e.LastModifiedById)
                .Including(e => e.Participants)
            );
    }

    private static Entry CreateWithDefaults()
        => 
            Entry.Create(
                Guid.NewGuid(), 
                [
                    User.Create("Andret2137", "andret2137@gmail.com"),
                    User.Create("KamilS", "kamil.s@gmail.com"),
                ]
            );
}
