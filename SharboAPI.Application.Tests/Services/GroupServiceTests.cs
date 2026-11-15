using FluentValidation;
using FluentValidation.Results;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Application.Services;
using SharboAPI.Domain.Enums;

namespace SharboAPI.Application.Tests.Services;

// xUnit creates a new instance per test, so every test gets fresh mocks.
public class GroupServiceTests
{
	private const string CurrentUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";
	private const string OtherUserId = "fa4e74f5c58448a5acba";

	private readonly Mock<IGroupRepository> _groupRepo = new();
	private readonly Mock<IRoleRepository> _roleRepo = new();
	private readonly Mock<IValidator<CreateGroupRequest>> _createValidator = new();
	private readonly Mock<IValidator<UpdateGroupRequest>> _updateValidator = new();
	private readonly Mock<ICurrentUser> _currentUser = new();
	private readonly GroupService _service;
	private static readonly CancellationToken CancellationToken = CancellationToken.None;

	public GroupServiceTests()
	{
		_currentUser.Setup(u => u.UserId).Returns(CurrentUserId);
		_createValidator
			.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateGroupRequest>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ValidationResult());
		_updateValidator
			.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateGroupRequest>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ValidationResult());
		_roleRepo
			.Setup(r => r.GetByRoleTypeAsync(It.IsAny<RoleType>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((RoleType type, CancellationToken _) => Role.Create(type, type.ToString()));

		_service = new GroupService(
			_groupRepo.Object,
			_roleRepo.Object,
			_createValidator.Object,
			_updateValidator.Object,
			_currentUser.Object);
	}

	#region GetMineAsync

	[Fact]
	public async Task GetMineAsync_ForSignedInUser_ReturnsOnlyGroupsFromRepositoryForCurrentUser()
	{
		// Arrange
		var groups = new List<Group> { CreateGroupWithMembers("A", CurrentUserId), CreateGroupWithMembers("B", CurrentUserId) };
		_groupRepo.Setup(r => r.GetAllByUserIdAsync(CurrentUserId, CancellationToken)).ReturnsAsync(groups);

		// Act
		var result = await _service.GetMineAsync(CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Select(g => g.Name).Should().Equal("A", "B");
		_groupRepo.Verify(r => r.GetAllByUserIdAsync(CurrentUserId, CancellationToken), Times.Once);
	}

	#endregion

	#region GetById

	[Fact]
	public async Task GetById_ForMember_ReturnsGroup()
	{
		// Arrange
		var group = CreateGroupWithMembers("Test", CurrentUserId, OtherUserId);
		_groupRepo.Setup(r => r.GetById(group.Id, CancellationToken)).ReturnsAsync(group);

		// Act
		var result = await _service.GetById(group.Id, CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value!.Name.Should().Be("Test");
		result.Value.Participants.Should().HaveCount(2);
	}

	[Fact]
	public async Task GetById_ForNonMember_ReturnsNotFoundResult()
	{
		// Arrange
		var group = CreateGroupWithMembers("Someone else's", OtherUserId);
		_groupRepo.Setup(r => r.GetById(group.Id, CancellationToken)).ReturnsAsync(group);

		// Act
		var result = await _service.GetById(group.Id, CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
	}

	[Fact]
	public async Task GetById_ForMissingGroup_ReturnsNotFoundResult()
	{
		// Act
		var result = await _service.GetById(Guid.NewGuid(), CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
	}

	#endregion

	#region AddAsync

	[Fact]
	public async Task AddAsync_ForCreateGroupRequest_MakesCurrentUserCreatorAndAdmin()
	{
		// Arrange
		var request = new CreateGroupRequest("Test", "https://example.com/image.jpg");
		_groupRepo.Setup(r => r.AddAsync(It.IsAny<Group>(), CancellationToken))
			.ReturnsAsync((Group g, CancellationToken _) => g.Id);

		// Act
		var result = await _service.AddAsync(request, CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		_groupRepo.Verify(r => r.AddAsync(
			It.Is<Group>(g => g.Name == request.Name
			                  && g.ImagePath == request.ImagePath
			                  && g.CreatedById == CurrentUserId
			                  && g.GroupParticipants.Count == 1
			                  && g.GroupParticipants[0].UserId == CurrentUserId
			                  && g.GroupParticipants[0].GroupParticipantRoles.Any(r => r.Role.RoleType == RoleType.Admin)),
			CancellationToken), Times.Once);
	}

	[Fact]
	public async Task AddAsync_ForCreateGroupRequestWithParticipants_AddsEachOtherUserOnceAndSkipsCreator()
	{
		// Arrange
		var request = new CreateGroupRequest("Test", null, [OtherUserId, OtherUserId, CurrentUserId]);
		Group? saved = null;
		_groupRepo.Setup(r => r.AddAsync(It.IsAny<Group>(), CancellationToken))
			.Callback((Group g, CancellationToken _) => saved = g)
			.ReturnsAsync((Group g, CancellationToken _) => g.Id);

		// Act
		await _service.AddAsync(request, CancellationToken);

		// Assert
		saved!.GroupParticipants.Select(p => p.UserId).Should().BeEquivalentTo([CurrentUserId, OtherUserId]);
	}

	#endregion

	#region UpdateAsync

	[Fact]
	public async Task UpdateAsync_ForMember_UpdatesAndReturnsGroup()
	{
		// Arrange
		var group = CreateGroupWithMembers("Test", CurrentUserId);
		var request = new UpdateGroupRequest("Test new", "https://example.com/image.jpg");
		_groupRepo.Setup(r => r.GetById(group.Id, CancellationToken)).ReturnsAsync(group);

		// Act
		var result = await _service.UpdateAsync(group.Id, request, CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value!.Name.Should().Be(request.Name);
		result.Value.ImagePath.Should().Be(request.ImagePath);
		group.LastModifiedById.Should().Be(CurrentUserId);
		_groupRepo.Verify(r => r.SaveChangesAsync(CancellationToken), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_ForNonMember_ReturnsNotFoundResultAndDoesNotSave()
	{
		// Arrange
		var group = CreateGroupWithMembers("Test", OtherUserId);
		_groupRepo.Setup(r => r.GetById(group.Id, CancellationToken)).ReturnsAsync(group);

		// Act
		var result = await _service.UpdateAsync(group.Id, new UpdateGroupRequest("Hacked", "https://x.y/z.jpg"),
			CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
		group.Name.Should().Be("Test");
		_groupRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
	}

	#endregion

	#region DeleteAsync

	[Fact]
	public async Task DeleteAsync_ForMember_DeletesGroup()
	{
		// Arrange
		var group = CreateGroupWithMembers("Test", CurrentUserId);
		_groupRepo.Setup(r => r.GetById(group.Id, CancellationToken)).ReturnsAsync(group);

		// Act
		var result = await _service.DeleteAsync(group.Id, CancellationToken);

		// Assert
		result.IsSuccess.Should().BeTrue();
		_groupRepo.Verify(r => r.DeleteAsync(group, CancellationToken), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_ForNonMember_ReturnsNotFoundResultAndDoesNotDelete()
	{
		// Arrange
		var group = CreateGroupWithMembers("Test", OtherUserId);
		_groupRepo.Setup(r => r.GetById(group.Id, CancellationToken)).ReturnsAsync(group);

		// Act
		var result = await _service.DeleteAsync(group.Id, CancellationToken);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
		_groupRepo.Verify(r => r.DeleteAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	#endregion

	#region Test_Factory_Methods

	private static Group CreateGroupWithMembers(string name, params string[] userIds)
	{
		var participantRole = Role.Create(RoleType.Participant, "Participant");
		var participants = userIds
			.Select(userId => GroupParticipant.Create(userId, [GroupParticipantRole.Create(participantRole)]))
			.ToList();

		return Group.Create(name, userIds[0], null, participants);
	}

	#endregion
}
