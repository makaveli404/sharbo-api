using SharboAPI.Application.DTO.Meme;
using FluentValidation;

namespace SharboAPI.Application.Validators.Meme;

public class UpdateMemeRequestValidator : AbstractValidator<UpdateMemeRequest>
{
    private const short TEXT_MAX_LENGTH = 70;

    public UpdateMemeRequestValidator()
    {
        RuleFor(req => req.Text)
            .NotEmpty()
            .WithMessage("Text for meme cannot be empty if given")
            .MaximumLength(TEXT_MAX_LENGTH)
            .WithMessage($"Maximum length for meme text is { TEXT_MAX_LENGTH }")
            .When(req => req is not null);
    }
}