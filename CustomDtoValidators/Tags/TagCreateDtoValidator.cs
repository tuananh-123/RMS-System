using FluentValidation;
using RMS.Dtos.Tags.Create;

namespace RMS.CustomDtoValidators.Tags;

public class TagCreateDtoValidator : AbstractValidator<TagCreateDto>
{
    public TagCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.PropertyColor).NotNull().NotEmpty().WithMessage("Property's color is required");
    }

}