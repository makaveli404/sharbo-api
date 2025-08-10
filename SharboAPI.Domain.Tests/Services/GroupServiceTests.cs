using FluentValidation;
using FluentValidation.Results;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Application.Services;
using SharboAPI.Domain.Enums;

namespace SharboAPI.Domain.Tests.Services;

public class GroupServiceTests
{
	private readonly Mock<IGroupRepository> _groupRepo = new();
	private readonly Mock<IRoleRepository> _roleRepo = new();
	private readonly Mock<IValidator<CreateGroup>> _createValidator = new();
	private readonly Mock<IValidator<UpdateGroup>> _updateValidator = new();
	private readonly GroupService _service;
	private static readonly CancellationToken CancellationToken = CancellationToken.None;

	public GroupServiceTests()
	{
		_service = new GroupService(
			_groupRepo.Object,
			_roleRepo.Object,
			_createValidator.Object,
			_updateValidator.Object);
	}

	[Fact]
	public async Task GetById_ReturnsGroup_WhenFound()
	{
		// Arrange
		const string name = "Test";
		const string imagePath = "https://example.com/image.jpg";
		var id = Guid.NewGuid();
		var createdById = Guid.NewGuid();
		var group = Group.Create(name, createdById, imagePath);

		_groupRepo.Setup(r => r.GetById(id, CancellationToken))
			.ReturnsAsync(group);

		// Act
		var actual = await _service.GetById(id, CancellationToken);

		// Assert
		Assert.Equal(name, actual.Value?.Name);
		Assert.Equal(imagePath, actual.Value?.ImagePath);
		Assert.Equal(createdById, actual.Value?.CreatedById);
	}

	[Fact]
	public async Task AddAsync_CreatesGroupAndReturnsId()
	{
		// Arrange
		const string name = "Test";
		const string imagePath = "https://example.com/image.jpg";
		var dto = new CreateGroup(name, imagePath);
		var role = Role.Create(RoleType.Admin, "Admin");

		_groupRepo.Setup(r => r.AddAsync(It.IsAny<Group>(), CancellationToken))
			.ReturnsAsync((Group g, CancellationToken _) => g.Id);
		_roleRepo.Setup(r => r.GetByRoleTypeAsync(It.IsAny<RoleType>(), CancellationToken))
			.ReturnsAsync(role);
		// Act
		var result = await _service.AddAsync(dto, CancellationToken);

		// Assert
		Assert.NotNull(result);
		_groupRepo.Verify(r => r.AddAsync(
			It.Is<Group>(g => g.Name == dto.Name
			                  && g.ImagePath == dto.ImagePath),
			CancellationToken), Times.Once);
	}

	[Fact]
	public async Task UpdateAsync_UpdatesAndReturnsGroup()
	{
		// Arrange
		const string name = "Test";
		const string newName = "Test new";
		const string newImagePath = "https://example.com/image.jpg";
		var id = Guid.NewGuid();
		var original = Group.Create(name, id, null, new List<GroupParticipant>());
		var dto = new UpdateGroup(newName, newImagePath);

		_groupRepo.Setup(r => r.GetById(id, CancellationToken))
			.ReturnsAsync(original);
		_updateValidator
			.Setup(v => v.ValidateAsync(dto, CancellationToken))
			.ReturnsAsync(new ValidationResult());
		_groupRepo.Setup(r => r.SaveChangesAsync(CancellationToken))
			.Returns(Task.CompletedTask);

		// Act
		var updated = await _service.UpdateAsync(id, dto, CancellationToken);

		// Assert
		Assert.NotNull(updated);
		Assert.Equal(dto.Name, updated.Value?.Name);
		Assert.Equal(dto.ImagePath, updated.Value?.ImagePath);
		_groupRepo.Verify(r => r.SaveChangesAsync(CancellationToken), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_CallsRepository()
	{
		// Arrange
		var id = Guid.NewGuid();

		// Act
		await _service.DeleteAsync(id, CancellationToken);

		// Assert
		_groupRepo.Verify(r => r.DeleteAsync(id, CancellationToken), Times.Once);
	}
}
