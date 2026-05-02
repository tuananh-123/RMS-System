using System;
using System.Data.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.Dtos.Recipes;
using RMS.Entities;
using RMS.IService.IRecipes;

namespace RMS.Service.Recipes;

public class RecipePagingService(
    RMSDbContext context,
    IMapper mapper,
    ILogger<RecipePagingService> logger
) : IRecipePagingService
{
    private readonly RMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RecipePagingService> _logger = logger;

    public async Task<ServiceResult> GetRecipePagingAsync(int pageNumber, int pageSize)
    {
        try
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid page number ({PageNumber}) or page size ({PageSize}). Both must be greater than zero.", pageNumber, pageSize);
                return new ServiceResult(false, StatusCodes.Status400BadRequest, "Page number and page size must be greater than zero.");
            }

            var query = _context.Recipes
                .AsNoTracking()
                .Where(r => !r.Trash);  

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();   
             
            var dataDto = _mapper.Map<List<RecipeDto>>(data);

            var result = new RecipePagingResponseDto
            {
                Rows = dataDto,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return new ServiceResult(true, StatusCodes.Status200OK, "Recipes fetched successfully.", result);
        }
        catch (DbException dbEx)
        {
            _logger.LogError(dbEx, "A database error occurred while fetching paginated recipes.");
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "A database error occurred while fetching recipes.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching paginated recipes.");
            return new ServiceResult(false, StatusCodes.Status500InternalServerError, "An error occurred while fetching recipes.");
        }
    }
}