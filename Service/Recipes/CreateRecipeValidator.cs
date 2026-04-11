using RMS.Contants;
using RMS.Dtos;

namespace RMS.Service.Recipes;

public class CreateRecipeValidator
{
    public ValidationResult[] RecipeBusinessRules(RecipeCreateDto request)
    {
        var rules = new List<Func<RecipeCreateDto, ValidationResult?>>
                {
                    // các quy tắc nâng cao cho Recipe
                    r => r is { TotalCalories: < 3000 } ? null : new ValidationResult("TotalCalories", "Tổng calo của món ăn không được vượt quá 3000."),
                    r => r is { Serving: > 0 and < 100 } ? null : new ValidationResult("Serving", "Số lượng phục vụ phải từ 1 đến 99."),
                    r => r is { CookingTime: > -1 } ? null : new ValidationResult("CookingTime", "Thời gian nấu phải lớn hơn 0."),
                    r => !(r.Difficulty == Difficulty.Hard && r.CookingTime < 60) ? null : new ValidationResult("CookingTime", "Món ăn khó phải có thời gian nấu ít nhất 60 phút."),
                    r => r.Title.Length >= 5 ? null : new ValidationResult("Title", "Tiêu đề phải có ít nhất 5 kí tự.")
                };
        var errors = rules
                    .Select(rule => rule(request))
                    .Where(error => error != null)
                    .Cast<ValidationResult>()
                    .ToArray();
        return errors;
    }
}