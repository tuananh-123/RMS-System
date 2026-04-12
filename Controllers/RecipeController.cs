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
public class RecipeController(ICreateRecipeService service, ILogger<RecipeController> logger) : ControllerBase
{
    readonly ICreateRecipeService _service = service;
    readonly ILogger<RecipeController> _logger = logger;

    [Authorize]
    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddRecipeAsync(int userId, RecipeCreateDto recipe)
    {
        _logger.LogInformation("Creating recipe for user {userId}", userId);

        var userIdFromClaims = GetUserIdFromClaims();
        if (userIdFromClaims == null || userIdFromClaims != userId)
        {
            _logger.LogWarning("Unauthorized attempt to create recipe for user {userId}", userId);
            return Forbid();
        }

        var result = await _service.ExecuteAsync(userId, recipe);
        _logger.LogInformation("Create recipe result for user {userId}: {result}", userId, result.Success ? "Success" : $"Failure - {result.Message}");
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
            return result.Code == StatusCodes.Status201Created ? CreatedAtAction(nameof(AddRecipeAsync), new { id = result.Data }, result) : Ok(result);
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