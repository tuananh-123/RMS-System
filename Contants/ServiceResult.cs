using Microsoft.AspNetCore.Mvc;

namespace RMS.Contants;

public class ServiceResult(bool success, int code, string? message = null, object? data = null)
{
    public bool Success { get; set; } = success;
    public int Code { get; set; } = code;
    public string? Message { get; set; } = message;
    public object? Data { get; set; } = data;

    public IActionResult OK() => new OkObjectResult(this);  
    public IActionResult Created(string location) => new CreatedResult(location, this);
    public IActionResult Conflict() => new ConflictObjectResult(this);
    public IActionResult BadRequest() => new BadRequestObjectResult(this);
    public IActionResult Forbidden() => new ForbidResult();
    public IActionResult NotFound() => new NotFoundObjectResult(this);
    public IActionResult Unauthorized() => new UnauthorizedObjectResult(this);
    public IActionResult StatusCode(int code) => new ObjectResult(this) { StatusCode = code };
    
}

