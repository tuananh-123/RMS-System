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
    public async Task<IActionResult> GetRecipeAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching recipes for page {pageNumber} with size {pageSize}", pageNumber, pageSize);

        var result = await _recipePagingService.GetRecipePagingAsync(pageNumber, pageSize);
        return result.OK();
    }

    [AllowAnonymous]
    [HttpGet("get/detail/{recipeId}")]
    public async Task<IActionResult> GetRecipeDetailAsync(int recipeId)
    {
        _logger.LogInformation("Fetching details for recipe {recipeId}", recipeId);

        var result = await _recipeDetailService.GetRecipeDetailAsync(recipeId);
        return result.OK();
    }

    // [Authorize]
    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddRecipeAsync(int userId, RecipeCreateDto recipe)
    {
        _logger.LogInformation("Creating recipe for user {userId}", userId);

        // var userIdFromClaims = GetUserIdFromClaims();
        // if (userIdFromClaims == null || userIdFromClaims != userId)
        // {
        //     _logger.LogWarning("Unauthorized attempt to create recipe for user {userId}", userId);
        //     return Forbid();
        // }

        var result = await _createService.ExecuteAsync(userId, recipe);
        _logger.LogInformation("Create recipe result for user {userId}: {result}", userId, result.Success ? "Success" : $"Failure - {result.Message}");
        return ToActionResult(result);
    }

    [HttpPut("update/{recipeId}/by/{userId}")]
    public async Task<IActionResult> UpdateRecipeAsync(int recipeId, int userId, RecipeUpdateDto recipe)
    {
        _logger.LogInformation("Updating recipe {recipeId} for user {userId}", recipeId, userId);

        // var userIdFromClaims = GetUserIdFromClaims();
        // if (userIdFromClaims == null || userIdFromClaims != userId)
        // {
        //     _logger.LogWarning("Unauthorized attempt to create recipe for user {userId}", userId);
        //     return Forbid();
        // }

        var result = await _updateService.ExecuteSync(userId, recipeId, recipe);
        _logger.LogInformation("Update recipe {recipeId} result for user {userId}: {result}", recipeId, userId, result.Success ? "Success" : $"Failure - {result.Message}");
        return ToActionResult(result);
    }

    private int? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim != null)
        {
            return int.Parse(userIdClaim);
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