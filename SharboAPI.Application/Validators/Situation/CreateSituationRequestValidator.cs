using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Situation;

namespace SharboAPI.Application.Validators.Situation;

public class CreateSituationRequestValidator : AbstractValidator<CreateSituationRequest>
{
    private const short TEXT_MAX_LENGTH = 70;

    public CreateSituationRequestValidator(IGroupRepository groupRepository)
    {
        RuleFor(req => req.GroupId)
            .MustAsync(groupRepository.IsExistById)
            .WithMessage("Group for given ID does not exist");

        RuleFor(req => req.Text)
            .NotEmpty()
            .WithMessage("Text for situation cannot be empty if given")
            .MaximumLength(TEXT_MAX_LENGTH)
            .WithMessage($"Maximum length for situation text is {TEXT_MAX_LENGTH}")
            .When(req => req is not null);
    }
}
