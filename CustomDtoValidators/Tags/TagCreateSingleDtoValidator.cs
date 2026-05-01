using FluentValidation;
using RMS.Dtos.Tags.Create;

namespace RMS.CustomDtoValidators.Tags;

public class TagCreateSingleDtoValidator : AbstractValidator<TagCreateSingleDto>
{
    public TagCreateSingleDtoValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.PropertyColor).NotNull().NotEmpty().WithMessage("Property's color is required");
    }

}