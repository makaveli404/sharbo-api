using FluentValidation;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Application.Validators.Group;

public class UpdateGroupDtoValidator : AbstractValidator<UpdateGroup>
{
	public UpdateGroupDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.WithErrorCode("Group name is required")
			.MaximumLength(50)
			.WithMessage("Group name is too long")
			.NotNull();

		RuleFor(x => x.ImagePath)
			.Must(path => string.IsNullOrEmpty(path) || Uri.IsWellFormedUriString(path, UriKind.Absolute))
			.WithMessage("ImagePath must be a valid URL");
	}
}
