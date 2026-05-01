using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RMS.Contants;
using RMS.Dtos.Auths;

namespace RMS.Service.Auths;

public class LoginUserService(
    RMSDbContext dbContext,
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    ILogger<LoginUserService> logger,
    IConfiguration configuration,
    JwtTokenService jwtTokenService,
    
    IMapper mapper) : BaseService(dbContext, mapper)
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly ILogger<LoginUserService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly JwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<(bool, string, AuthResponseDto?)> ExecuteAsync(LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Cannot find user with email: {email}", request.Email);
            return (false, "Tài khoản hoặc mật khẩu không đúng!", null);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Password is incorrectd.");
            return (false, "Tài khoản hoặc mật khẩu không đúng!", null);
        }

        var response = await _jwtTokenService.GenerateTokenPairAsync(user);
        return (true, "Đăng nhập thành công!", response);
    }

    
}