
using RMS.Contants;
using RMS.Entities;

namespace RMS.IService.IRecipes;

public interface ICreateRecipeValidator
{
    ValidationResult[] ValidateBusinessRules(Recipe request);
}