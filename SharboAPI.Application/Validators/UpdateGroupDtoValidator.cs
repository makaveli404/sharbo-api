using FluentValidation;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Application.Validators;

public class UpdateGroupDtoValidator : AbstractValidator<UpdateGroupDto>
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
