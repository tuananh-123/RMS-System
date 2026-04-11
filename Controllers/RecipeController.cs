using RMS.Dtos;
using RMS.IService;
using Microsoft.AspNetCore.Mvc;
using RMS.Service.Recipes;

namespace RMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController(CreateRecipeService service) : ControllerBase
{
    readonly CreateRecipeService _service = service;

    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddRecipeAsync(int userId, RecipeCreateDto recipe)
    {
        var result = await _service.ExecuteAsync(userId, recipe);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // [HttpPut("update/{id}/by/{userId}")]
    // public async Task<IActionResult> UpdateRecipeAsync(int userId, int id, RecipeUpdateDto recipe)
    // {
    //     var result = await _service.cre(userId, id, recipe);
    //     return result.Success ? Ok(result) : BadRequest(result);
    // }
}