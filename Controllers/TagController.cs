using Microsoft.AspNetCore.Mvc;
using RMS.Dtos.Tags.Create;
using RMS.IService.ITags.ICreate;
using RMS.Service.Tags.Create;

namespace RMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController(
    ITagCreateService tagCreateSingleService,
    TagCreateByFileService tagCreateByFileService
) : BaseController
{
    private readonly ITagCreateService _tagCreateSingleService = tagCreateSingleService;
    private readonly TagCreateByFileService _tagCreateByFileService = tagCreateByFileService;

    [HttpPost("add/by/{userId}")]
    public async Task<IActionResult> AddSingleTagAsync(int userId, TagCreateDto request)
    {
        var serviceResult = await _tagCreateSingleService.ExecuteAsync(userId, request);
        return ToActionResult(serviceResult);
    }

    [HttpPost("add/by/{userId}/file")]
    public async Task<IActionResult> AddTagsFromFile(int userId, IFormFile file, CancellationToken ct)
    {
        var (success, code, message, data) = await _tagCreateByFileService.ExecuteAsync(userId, file, ct);        
        return ToActionResult(success, code, message, data);
    }
}