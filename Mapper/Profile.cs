using AutoMapper;
using RMS.Contants;
using RMS.Dtos;
using RMS.Dtos.Recipes;
using RMS.Entities;

namespace RMS.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region map Recipe to RecipeDetailDto
        CreateMap<Recipe, RecipeDetailDto>()
        .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
        .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TagForRecipes));

        CreateMap<TagForRecipe, TagDto>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.TagID))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Tag.Title))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Tag.Color));

        CreateMap<RecipeIngredient, IngredientDto>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Ingredient.Title))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.IngredientID));
        #endregion

    }
}