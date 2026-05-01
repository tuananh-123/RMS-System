using Microsoft.AspNetCore.Mvc;
using RMS.Dtos.Auths;
using RMS.Service.Auths;

namespace RMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    RegisterUserService registerUserService,
    LoginUserService loginUserService,
    LogoutUserService logoutUserService,
    RefreshTokenService refreshTokenService
) : BaseController
{
    private readonly RegisterUserService _registerUserService = registerUserService;
    private readonly LoginUserService _loginUserService = loginUserService;
    private readonly LogoutUserService _logoutUserService = logoutUserService;
    private readonly RefreshTokenService _refreshTokenService = refreshTokenService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var (success, message, data) = await _registerUserService.ExcuteAsync(request);
        if (!success)
            return BadRequest(new { message });

        return Ok(new { message, data });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var (success, message, data) = await _loginUserService.ExecuteAsync(request);
        if (!success)
            return Unauthorized(new { message });

        return Ok(new { message, data });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "RefreshToken is missing." });

        var (success, message) = await _logoutUserService.ExecuteAsync(dto.RefreshToken);
        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "RefreshToken is missing." });

        var (success, message, data) = await _refreshTokenService.ExecuteAsync(dto.RefreshToken);
        if (!success)
            return Unauthorized(new {message});

        return Ok(new {message, data});
    }

}