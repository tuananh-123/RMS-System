using AutoMapper;
using RMS.IService;

namespace RMS.Service;

public class IngredientService(RMSDbContext context, IMapper mapper) : BaseService<Ingredient>(context), IIngredientService
{
    private readonly IMapper _mapper = mapper;
}