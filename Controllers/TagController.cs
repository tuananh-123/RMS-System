using Microsoft.AspNetCore.Mvc;
using RMS.Dtos.Tags.Create;
using RMS.IService.ITags.ICreate;

namespace RMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController(
    ITagCreateSingleService tagCreateSingleService
) : BaseController
{
    private readonly ITagCreateSingleService _tagCreateSingleService = tagCreateSingleService;

    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddSingleTagAsync(int userId, TagCreateSingleDto request)
    {
        var serviceResult = await _tagCreateSingleService.ExecuteAsync(userId, request);
        return ToActionResult(serviceResult);
    }
}