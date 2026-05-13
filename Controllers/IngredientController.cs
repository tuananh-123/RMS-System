using Microsoft.AspNetCore.Mvc;
using RMS.Contants;
using RMS.Dtos.Ingredients;
using RMS.IService.IIngredients.ICreate;

namespace RMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController(
    IIngredientCreateSingleService ingredientCreateSingleService,
    IIngredientCreateByFileInputService ingredientCreateByFileInputService,
    ILogger<IngredientController> logger
) : BaseController
{
    private readonly IIngredientCreateSingleService _ingredientCreateSingleService = ingredientCreateSingleService;
    private readonly IIngredientCreateByFileInputService _ingredientCreateByFileInputService = ingredientCreateByFileInputService;
    private readonly ILogger<IngredientController> _logger = logger;

    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddSingleIngredientAsync(int userId, IngredientCreateDto request)
    {
        var actionResult = await _ingredientCreateSingleService.ExecuteAsync(userId, request);
        return ToActionResult(actionResult);
    }

    [HttpPost("add/by/{userId}/file")]
    public async Task<IActionResult> AddIngredientsFromFileAsync(int userId, IFormFile file)
    {
        var actionResult = await _ingredientCreateByFileInputService.ExecuteAsync(userId, file);
        return ToActionResult(actionResult);
    }
}