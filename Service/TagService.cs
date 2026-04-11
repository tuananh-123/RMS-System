using AutoMapper;
using RMS.IService;


namespace RMS.Service;

public class TagService(RMSDbContext context, IMapper mapper) : BaseService<Tag>(context), ITagService
{
    private readonly IMapper _mapper = mapper;
}