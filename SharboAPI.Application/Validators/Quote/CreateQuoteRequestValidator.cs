using SharboAPI.Application.DTO.Quote;
using FluentValidation;

namespace SharboAPI.Application.Validators.Quote;

public class CreateQuoteRequestValidator : AbstractValidator<CreateQuoteRequest>
{
    private const short TEXT_MAX_LENGTH = 70;

    public CreateQuoteRequestValidator()
    {
        RuleFor(req => req.Text)
            .NotEmpty()
            .WithMessage("Text for quote cannot be empty if given")
            .MaximumLength(TEXT_MAX_LENGTH)
            .WithMessage($"Maximum length for quote text is { TEXT_MAX_LENGTH }");
    }
}
