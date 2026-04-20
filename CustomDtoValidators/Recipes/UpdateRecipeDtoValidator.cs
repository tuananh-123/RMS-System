using FluentValidation;
using RMS.Dtos;

namespace RMS.CustomDtoValidators.Recipes;

public class UpdateRecipeDtoValidator : AbstractValidator<RecipeUpdateDto>
{
    public UpdateRecipeDtoValidator()
    {
        RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Nation).IsInEnum().WithMessage("Invalid nation value.");
        RuleFor(x => x.Cuisine).NotNull().NotEmpty().WithMessage("At least one cuisine is required.");
        RuleFor(x => x.Serving).GreaterThan(0).WithMessage("Serving must be greater than 0.");
        RuleFor(x => x.TotalCalories).GreaterThanOrEqualTo(0).WithMessage("Total calories must be non-negative.");
        RuleFor(x => x.CookingTime).GreaterThan(0).WithMessage("Cooking time must be greater than 0.");
        RuleFor(x => x.Difficulty).IsInEnum().WithMessage("Invalid difficulty value.");
        RuleFor(x => x.ImageCover).NotNull().NotEmpty().WithMessage("Image cover is required.");
        RuleFor(x => x.VideoUrl).NotNull().NotEmpty().WithMessage("Video URL is required.");
        RuleFor(x => x.Tags).NotNull().NotEmpty().WithMessage("At least one tag is required.");
        RuleFor(x => x.SearchKeyword).NotNull().WithMessage("Search keyword is required.");
        RuleFor(x => x.RecipeIngredients).NotNull().NotEmpty().WithMessage("At least one ingredient is required.");
        RuleFor(x => x.LastedVersion).GreaterThan(0).WithMessage("Lasted version must be greater than 0.");
    }
}