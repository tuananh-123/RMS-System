using RMS.Dtos;
using RMS.IService;
using Microsoft.AspNetCore.Mvc;
using RMS.Service.Recipes;
using RMS.IService.IRecipes;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RMS.Contants;
using RMS.Dtos.Recipes;

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
    [HttpGet("get/list/{page}/{pageSize}/{sortBy}")]
    public async Task<IActionResult> GetRecipeByPaging(int page, int pageSize, string sortBy, [FromQuery]RecipeFilterDto filter, CancellationToken ct)
    {
        var (success, message, data) = await _recipePagingService.Execute(sortBy, filter, ct, page, pageSize);
        if (!success)
            return BadRequest(new {message});
        return Ok(new {message, data});
    }

    [AllowAnonymous]
    [HttpGet("get/detail/{recipeId}")]
    public async Task<IActionResult> GetRecipeDetailAsync(int recipeId)
    {
        var (success, message, data) = await _recipeDetailService.GetRecipeDetailAsync(recipeId);
        if (!success)
            return NotFound(new { message });
        return Ok(new { message, data });
    }

    [Authorize]
    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddRecipeAsync(string userId, RecipeCreateDto recipe)
    {
        var userIdFromClaims = GetUserIdFromClaims();
        if (userIdFromClaims == null || userIdFromClaims != userId)
        {
            _logger.LogWarning("Unauthorized attempt to create recipe for user {userId}", userId);
            return Forbid();
        }

        var result = await _createService.ExecuteAsync(userId, recipe);
        return ToActionResult(result);
    }
    
    [Authorize]
    [HttpPut("update/{recipeId}/by/{userId}")]
    public async Task<IActionResult> UpdateRecipeAsync(int recipeId, string userId, RecipeUpdateDto recipe)
    {

        var userIdFromClaims = GetUserIdFromClaims();
        if (userIdFromClaims == null || userIdFromClaims != userId)
        {
            _logger.LogWarning("Unauthorized attempt to create recipe for user {userId}", userId);
            return Forbid();
        }

        var result = await _updateService.ExecuteSync(userId, recipeId, recipe);
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