using SharboAPI.Application.DTO.Meme;
using SharboAPI.Application.Abstractions.Repositories;
using FluentValidation;

namespace SharboAPI.Application.Validators.Meme;

public class CreateMemeRequestValidator : AbstractValidator<CreateMemeRequest>
{
	private const short TEXT_MAX_LENGTH = 70;

	public CreateMemeRequestValidator(IGroupRepository groupRepository)
	{
		RuleFor(req => req.GroupId)
			.MustAsync(async (id, token) => await groupRepository.GetById(id, token) is not null)
			.WithMessage("Group for given ID does not exist");

		RuleFor(req => req.Text)
			.NotEmpty()
			.WithMessage("Text for meme cannot be empty if given")
			.MaximumLength(TEXT_MAX_LENGTH)
			.WithMessage($"Maximum length for meme text is { TEXT_MAX_LENGTH }")
			.When(req => req is not null);

		RuleFor(req => req.ImagePath)
			.NotNull()
			.WithMessage("Image path must be given")
			.NotEmpty()
			.WithMessage("Image path cannot be empty string")
			.Must(path => Uri.IsWellFormedUriString(path, UriKind.Absolute))
			.WithMessage("ImagePath must own a valid URL format");
	}
}
