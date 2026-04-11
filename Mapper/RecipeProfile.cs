using AutoMapper;
using RMS.Dtos;
using RMS.Entities;

public class RecipeProfile: Profile
{
    public RecipeProfile()
    {
        CreateMap<RecipeUpdateDto, Recipe>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.LastedVersion, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeHistories, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.TagForRecipes, opt => opt.Ignore());


        CreateMap<Recipe, RecipeHistory>()
            .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.LastedVersion))
            .ForMember(dest => dest.RecipeID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Cuisine, opt => opt.MapFrom(src => src.Cuisine.ToArray()));
        
        CreateMap<RecipeCreateDto, Recipe>()
            .ForMember(dest => dest.TotalCalories, opt => opt.Ignore())
            .ForMember(dest => dest.Views, opt => opt.Ignore())
            .ForMember(dest => dest.Rating, opt => opt.Ignore())
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.LastedVersion, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeHistories, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.TagForRecipes, opt => opt.Ignore())
            ;
            // .ForMember(dest => dest.Nation, opt => opt.MapFrom(src => src.Nation));
    }
}