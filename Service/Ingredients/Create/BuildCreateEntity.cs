using AutoMapper;
using RMS.Dtos.Ingredients;
using RMS.Entities;

namespace RMS.Service.Ingredients.Create;

public class IngredientCreationBuilder(IMapper mapper)
{
    private readonly IMapper _mapper = mapper;
    public Ingredient BuildEntity(int userId, IngredientCreateDto request)
    {
        var ingredient = _mapper.Map<Ingredient>(request);
        ingredient.CreatedBy = userId.ToString();
        NormalizeProperties(ingredient);
        return ingredient;
    }

    private static void NormalizeProperties(Ingredient entity)
    {
        entity.Title = entity.Title.Trim();
        entity.Information = entity.Information.Trim();
        entity.SearchKeyword!.Keywords = [.. entity.SearchKeyword.Keywords.Select(k => k.Trim())];
        entity.SearchKeyword.Hashtags = [.. entity.SearchKeyword.Hashtags.Select(h => h.Trim())];
    }
}