using Microsoft.Extensions.Logging.Abstractions;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.GroupParticipant;
using SharboAPI.Application.Services;
using SharboAPI.Domain.Enums;

namespace SharboAPI.Application.Tests.Services;

// xUnit creates a new instance per test, so every test gets fresh mocks.
public class GroupParticipantServiceTests
{
	private const string CurrentUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";
	private const string OtherUserId = "fa4e74f5c58448a5acba";

	private readonly Mock<IGroupParticipantRepository> _participantRepo = new();
	private readonly Mock<IRoleRepository> _roleRepo = new();
	private readonly Mock<ICurrentUser> _currentUser = new();
	private readonly GroupParticipantService _service;
	private static readonly CancellationToken CancellationToken = CancellationToken.None;

	public GroupParticipantServiceTests()
	{
		_currentUser.Setup(u => u.UserId).Returns(CurrentUserId);
		_roleRepo
			.Setup(r => r.GetByRoleTypeAsync(It.IsAny<RoleType>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((RoleType type, CancellationToken _) => Role.Create(type, type.ToString()));

		_service = new GroupParticipantService(
			_participantRepo.Object,
			_roleRepo.Object,
			_currentUser.Object,
			NullLogger<GroupParticipantService>.Instance);
	}

	#region GetGroupParticipantsByGroupIdAsync

	[Fact]
	public async Task GetGroupParticipantsByGroupIdAsync_ForMember_ReturnsParticipants()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		SetupCurrentUserAsMemberOf(groupId);
		_participantRepo.Setup(r => r.GetByGroupIdAsync(groupId, CancellationToken))
			.ReturnsAsync([CreateParticipant(groupId, CurrentUserId), CreateParticipant(groupId, OtherUserId)]);

		// Act
		var result = await _service.GetGroupParticipantsByGroupIdAsync(groupId, CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Should().HaveCount(2);
	}

	[Fact]
	public async Task GetGroupParticipantsByGroupIdAsync_ForNonMember_ReturnsNotFoundResult()
	{
		// Arrange
		var groupId = Guid.NewGuid();

		// Act
		var result = await _service.GetGroupParticipantsByGroupIdAsync(groupId, CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
		_participantRepo.Verify(r => r.GetByGroupIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	#endregion

	#region AddAsync

	[Fact]
	public async Task AddAsync_ForMember_AddsNewUsersAndSkipsExistingMembers()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		SetupCurrentUserAsMemberOf(groupId);

		// Act
		var result = await _service.AddAsync(groupId, [OtherUserId, OtherUserId, CurrentUserId], CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		_participantRepo.Verify(r => r.AddAsync(
			It.Is<GroupParticipant>(p => p.UserId == OtherUserId && p.GroupId == groupId), CancellationToken), Times.Once);
		_participantRepo.Verify(r => r.AddAsync(
			It.Is<GroupParticipant>(p => p.UserId == CurrentUserId), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task AddAsync_ForNonMember_ReturnsNotFoundResultAndAddsNobody()
	{
		// Act
		var result = await _service.AddAsync(Guid.NewGuid(), [OtherUserId], CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
		_participantRepo.Verify(r => r.AddAsync(It.IsAny<GroupParticipant>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	#endregion

	#region DeleteAsync

	[Fact]
	public async Task DeleteAsync_ForMember_DeletesOnlyWithinThatGroup()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		Guid[] participantIds = [Guid.NewGuid()];
		SetupCurrentUserAsMemberOf(groupId);

		// Act
		var result = await _service.DeleteAsync(groupId, participantIds, CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		_participantRepo.Verify(r => r.DeleteAsync(groupId, participantIds, CancellationToken), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_ForNonMember_ReturnsNotFoundResultAndDeletesNothing()
	{
		// Act
		var result = await _service.DeleteAsync(Guid.NewGuid(), [Guid.NewGuid()], CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		_participantRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()),
			Times.Never);
	}

	#endregion

	#region UpdateRolesAsync

	[Fact]
	public async Task UpdateRolesAsync_ForParticipantOfAnotherGroup_ReturnsNotFoundResultAndDoesNotSave()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		SetupCurrentUserAsMemberOf(groupId);
		var participantOfOtherGroup = CreateParticipant(Guid.NewGuid(), OtherUserId);
		_participantRepo.Setup(r => r.GetByIdAsync(participantOfOtherGroup.Id, CancellationToken))
			.ReturnsAsync(participantOfOtherGroup);

		// Act
		var result = await _service.UpdateRolesAsync(groupId, participantOfOtherGroup.Id,
			new UpdateGroupParticipantRolesRequest(["Admin"]), CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
		_participantRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task UpdateRolesAsync_ForNonMember_ReturnsNotFoundResult()
	{
		// Act
		var result = await _service.UpdateRolesAsync(Guid.NewGuid(), Guid.NewGuid(),
			new UpdateGroupParticipantRolesRequest(["Admin"]), CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		_participantRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task UpdateRolesAsync_ForParticipantOfSameGroup_SetsRequestedRoles()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		SetupCurrentUserAsMemberOf(groupId);
		var participant = CreateParticipant(groupId, OtherUserId);
		var adminRole = Role.Create(RoleType.Admin, "Admin");
		_participantRepo.Setup(r => r.GetByIdAsync(participant.Id, CancellationToken)).ReturnsAsync(participant);
		_roleRepo.Setup(r => r.GetByRoleNameAsync("Admin", CancellationToken)).ReturnsAsync(adminRole);

		// Act
		var result = await _service.UpdateRolesAsync(groupId, participant.Id,
			new UpdateGroupParticipantRolesRequest(["Admin"]), CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		participant.GroupParticipantRoles.Select(r => r.Role).Should().Equal(adminRole);
		_participantRepo.Verify(r => r.SaveChangesAsync(CancellationToken), Times.Once);
	}

	#endregion

	#region Test_Factory_Methods

	private void SetupCurrentUserAsMemberOf(Guid groupId)
		=> _participantRepo
			.Setup(r => r.GetByUserIdAndGroupIdAsync(CurrentUserId, groupId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(CreateParticipant(groupId, CurrentUserId));

	private static GroupParticipant CreateParticipant(Guid groupId, string userId)
		=> GroupParticipant.Create(groupId, userId, [GroupParticipantRole.Create(Role.Create(RoleType.Participant, "Participant"))]);

	#endregion
}
