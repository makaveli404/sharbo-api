using FluentValidation;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Application.Validators;

public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
{
	public CreateGroupDtoValidator()
	{
		RuleFor(x => x.Name).NotEmpty().WithErrorCode("Group name is required").MaximumLength(50).WithMessage("Group name is too long").NotNull();
		RuleFor(x => x.ImagePath)
			.Must(path => string.IsNullOrEmpty(path) || Uri.IsWellFormedUriString(path, UriKind.Absolute))
			.WithMessage("ImagePath must be a valid URL");
		RuleForEach(x => x.Participants)
			.ChildRules(p =>
			{
				p.RuleFor(x => x.UserId)
					.NotEqual(Guid.Empty).WithMessage("UserId is required");
			});
	}
}
