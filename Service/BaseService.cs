using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;

namespace RMS.Service;

public class BaseService(
    RMSDbContext dbContext,
    IMapper mapper
)
{
    public readonly RMSDbContext _dbContext = dbContext;
    public readonly IMapper _mapper = mapper;

    public ServiceResult MapDbException(DbUpdateException dbEx, string entityClass)
    {
        if (dbEx.InnerException is Npgsql.PostgresException pgEx)
        {
            if (pgEx.SqlState == "23505")
            {
                return new ServiceResult(false, StatusCodes.Status409Conflict, $"A {entityClass} with the same value already exists.");
            }
        }

        return new ServiceResult(false, StatusCodes.Status500InternalServerError, "Database operation failed");
    }
}