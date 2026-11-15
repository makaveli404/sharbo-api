using FluentValidation;
using SharboAPI.Application.DTO.User;

namespace SharboAPI.Application.Validators.User;

public class CreateProfileRequestValidator : AbstractValidator<CreateProfileRequest>
{
	private const short NICKNAME_MAX_LENGTH = 50;

	public CreateProfileRequestValidator()
	{
		RuleFor(req => req.Nickname)
			.NotEmpty()
			.WithMessage("Nickname is required")
			.MaximumLength(NICKNAME_MAX_LENGTH)
			.WithMessage($"Maximum length for nickname is { NICKNAME_MAX_LENGTH }");
	}
}
