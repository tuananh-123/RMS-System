using AutoMapper;
using Microsoft.OpenApi;
using RMS.Dtos;
using RMS.Dtos.Recipes;
using RMS.Entities;

namespace RMS.Mapper;

public class RecipeProfile: Profile
{
    public RecipeProfile()
    {
        CreateMap<Recipe, Recipe>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.TagForRecipes, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeHistories, opt => opt.Ignore());
           
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
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
          

        CreateMap<RecipeUpdateDto, Recipe>()
            .ForMember(dest => dest.TotalCalories, opt => opt.Ignore())
            .ForMember(dest => dest.Views, opt => opt.Ignore())
            .ForMember(dest => dest.Rating, opt => opt.Ignore())
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeHistories, opt => opt.Ignore())
            .ForMember(dest => dest.RecipeIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.TagForRecipes, opt => opt.Ignore())
            .ForMember(dest => dest.LastedVersion, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<Recipe, RecipeDto>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.CookingTime, opt => opt.MapFrom(src => src.CookingTime))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.Servings, opt => opt.MapFrom(src => src.Serving))
            .ForMember(dest => dest.Nation, opt => opt.MapFrom(src => src.Nation))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageCover));

        CreateMap<Recipe, RecipeDetailDto>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.CookingTime, opt => opt.MapFrom(src => src.CookingTime))
            .ForMember(dest => dest.Views, opt => opt.MapFrom(src => 214))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => 4.5))
            .ForMember(dest => dest.Servings, opt => opt.MapFrom(src => src.Serving))
            .ForMember(dest => dest.Nation, opt => opt.MapFrom(src => src.Nation.GetDisplayName().ToString()))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Difficulty.GetDisplayName().ToString()))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageCover))
            .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.VideoUrl))
            .ForMember(dest => dest.NumberOfVersions, opt => opt.MapFrom(src => src.LastedVersion))
            .ForMember(dest => dest.SearchKeyword, opt => opt.MapFrom(src => src.SearchKeyword))
            .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src => src.TotalCalories))
            .ForMember(dest => dest.Cuisine, opt => opt.MapFrom(src => src.Cuisine))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}