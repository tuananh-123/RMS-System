using Microsoft.AspNetCore.Mvc;
using RMS.Contants;

namespace RMS.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult ToActionResult(ServiceResult result)
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