using AutoMapper;
using RMS.Dtos.Ingredients;
using RMS.Entities;

namespace RMS.Mapper;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<IngredientCreateDto, Ingredient>();

        CreateMap<JSONDeserializeCreateIngredientDto, IngredientCreateDto>()
            .ForMember(dest => dest.SearchKeyword, opt => opt.Ignore());

    }
}