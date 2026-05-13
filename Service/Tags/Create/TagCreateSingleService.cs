using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.CustomDtoValidators.Tags;
using RMS.Dtos.Tags.Create;
using RMS.Entities;
using RMS.Helpers;
using RMS.IService.ITags.ICreate;

namespace RMS.Service.Tags.Create;

public class TagCreateSingleService(
    RMSDbContext dbContext,
    IMapper mapper,
    ILogger<TagCreateSingleService> logger,
    TagCreateDtoValidator validator
) : BaseService(dbContext, mapper), ITagCreateService
{
    private readonly ILogger<TagCreateSingleService> _logger = logger;
    private readonly TagCreateDtoValidator _validator = validator;
    public async Task<ServiceResult> ExecuteAsync(int userId, TagCreateDto request)
    {
        // step 1: validate reuqest
        var requestValidation = await _validator.ValidateAsync(request);
        if (!requestValidation.IsValid)
        {
            var errorMessages = requestValidation.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
            _logger.LogWarning("Validation failed for User {UserId}: {Errors}", userId, errorMessages);
            return new ServiceResult(false, StatusCodes.Status400BadRequest, "Some field are required", errorMessages);
        }

        // step 2: check title duplication
        var isDuplicated = await CheckTitleDuplicate(request.Title);
        if (isDuplicated)
        {
            _logger.LogWarning("Tag's title duplication for User {UserId}: {title}", userId, request.Title);
            return new ServiceResult(false, StatusCodes.Status409Conflict, "Title duplicated", request.Title);
        }

        // step 3; build entity
        var tag = BuildEntity(userId, request);

        return await PersistToDatabase(userId, tag);
    }
    private Tag BuildEntity(int userId, TagCreateDto request)
    {
        var tag = _mapper.Map<Tag>(request);
        tag.Slug = tag.Title.ToSlug();
        tag.CreatedBy = userId.ToString();
        return tag;
    }
    private async Task<bool> CheckTitleDuplicate(string title) => await _dbContext.Tags.AnyAsync(t => t.Title.Trim().ToLower() == title.Trim().ToLower());
    private async Task<ServiceResult> PersistToDatabase(int userId, Tag tag)
    {
        try
        {
            await _dbContext.AddAsync(tag);
            await _dbContext.SaveChangesAsync();
            return new ServiceResult(true, StatusCodes.Status201Created, "Tag created successfully.");
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error occurred while creating tag");
            return MapDbException(dbEx, "Tag");
        }
    }
}