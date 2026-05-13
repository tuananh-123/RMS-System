using AutoMapper;
using RMS.Dtos.Tags.Create;
using RMS.Entities;

namespace RMS.Mapper;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<TagCreateDto, Tag>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.PropertyColor.Trim()));
    }
}