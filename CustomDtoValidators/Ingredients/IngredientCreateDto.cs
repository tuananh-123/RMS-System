using FluentValidation;
using RMS.Dtos.Ingredients;

namespace RMS.CustomDtoValidators.Ingredients;

public class IngredientCreateDtoValidator : AbstractValidator<IngredientCreateDto>
{
    public IngredientCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Information).NotNull().NotEmpty().WithMessage("Information is required.");
        RuleFor(x => x.CaloriePer100Gram).GreaterThanOrEqualTo(0).WithMessage("Calorie per 100 gram must be non-negative.");
        RuleFor(x => x.SearchKeyword).NotNull().WithMessage("Search keyword is required.");
    }
}