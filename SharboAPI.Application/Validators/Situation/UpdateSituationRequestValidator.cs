using FluentValidation;
using SharboAPI.Application.DTO.Situation;

namespace SharboAPI.Application.Validators.Situation;

public sealed class UpdateSituationRequestValidator : AbstractValidator<UpdateSituationRequest>
{
    private const short TEXT_MAX_LENGTH = 70;

    public UpdateSituationRequestValidator()
    {
        RuleFor(req => req.Text)
            .NotEmpty()
            .WithMessage("Text for situation cannot be empty if given")
            .MaximumLength(TEXT_MAX_LENGTH)
            .WithMessage($"Maximum length for situation text is { TEXT_MAX_LENGTH }")
            .When(req => req is not null);
    }
}
