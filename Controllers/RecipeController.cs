using RMS.Dtos;
using RMS.IService;
using Microsoft.AspNetCore.Mvc;
using RMS.Service.Recipes;
using RMS.IService.IRecipes;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RMS.Contants;

namespace RMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecipeController(
    ICreateRecipeService createService,
    IUpdateRecipeService updateService,
    IRecipePagingService recipePagingService,
    IRecipeDetailService recipeDetailService,
    ILogger<RecipeController> logger) : ControllerBase
{
    private readonly ICreateRecipeService _createService = createService;
    private readonly IUpdateRecipeService _updateService = updateService;
    private readonly IRecipePagingService _recipePagingService = recipePagingService;
    private readonly IRecipeDetailService _recipeDetailService = recipeDetailService;
    private readonly ILogger<RecipeController> _logger = logger;

    [AllowAnonymous]
    [HttpGet("get/page/{pageNumber}/size/{pageSize}")]
    public async Task<IActionResult> GetRecipeByPaging(int pageNumber, int pageSize)
    {
        var result = await _recipePagingService.GetRecipePagingAsync(pageNumber, pageSize);
        return result.OK();
    }

    [AllowAnonymous]
    [HttpGet("get/detail/{recipeId}")]
    public async Task<IActionResult> GetRecipeDetailAsync(int recipeId)
    {
        var (success, message, data) = await _recipeDetailService.GetRecipeDetailFromDistributeCacheAsync(recipeId);
        if (!success)
            return NotFound(new { message });
        return Ok(new { message, data });
    }

    [Authorize]
    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddRecipeAsync(string userId, RecipeCreateDto recipe)
    {
#pragma warning disable CA1873 // Avoid potentially expensive logging
        _logger.LogInformation("Creating recipe for user {userId}", userId);
#pragma warning restore CA1873 // Avoid potentially expensive logging

        var userIdFromClaims = GetUserIdFromClaims();
        if (userIdFromClaims == null || userIdFromClaims != userId)
        {
            _logger.LogWarning("Unauthorized attempt to create recipe for user {userId}", userId);
            return Forbid();
        }

        var result = await _createService.ExecuteAsync(userId, recipe);
#pragma warning disable CA1873 // Avoid potentially expensive logging
        _logger.LogInformation("Create recipe result for user {userId}: {result}", userId, result.Success ? "Success" : $"Failure - {result.Message}");
#pragma warning restore CA1873 // Avoid potentially expensive logging
        return ToActionResult(result);
    }
    [Authorize]
    [HttpPut("update/{recipeId}/by/{userId}")]
    public async Task<IActionResult> UpdateRecipeAsync(int recipeId, string userId, RecipeUpdateDto recipe)
    {
#pragma warning disable CA1873 // Avoid potentially expensive logging
        _logger.LogInformation("Updating recipe {recipeId} for user {userId}", recipeId, userId);
#pragma warning restore CA1873 // Avoid potentially expensive logging

        var userIdFromClaims = GetUserIdFromClaims();
        if (userIdFromClaims == null || userIdFromClaims != userId)
        {
            _logger.LogWarning("Unauthorized attempt to create recipe for user {userId}", userId);
            return Forbid();
        }

        var result = await _updateService.ExecuteSync(userId, recipeId, recipe);
#pragma warning disable CA1873 // Avoid potentially expensive logging
        _logger.LogInformation("Update recipe {recipeId} result for user {userId}: {result}", recipeId, userId, result.Success ? "Success" : $"Failure - {result.Message}");
#pragma warning restore CA1873 // Avoid potentially expensive logging
        return ToActionResult(result);
    }

    private string? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim != null)
        {
            return userIdClaim.ToString();
        }

        return null;
    }

    private IActionResult ToActionResult(ServiceResult result)
    {
        if (result.Success)
        {
            return result.Code == StatusCodes.Status201Created
                ? StatusCode(StatusCodes.Status201Created, result)
                : Ok(result);
        }

        return result.Code switch
        {
            StatusCodes.Status409Conflict => Conflict(result),
            StatusCodes.Status400BadRequest => BadRequest(result),
            StatusCodes.Status403Forbidden => Forbid(),
            StatusCodes.Status404NotFound => NotFound(result),
            StatusCodes.Status401Unauthorized => Unauthorized(result),
            _ => StatusCode(result.Code, result)
        };
    }
}