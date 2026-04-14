using AutoMapper;
using RMS.IService;
using RMS.Entities;

namespace RMS.Service;

public class IngredientService(RMSDbContext context, IMapper mapper) : BaseService<Ingredient>(context), IIngredientService
{
    private readonly IMapper _mapper = mapper;
}