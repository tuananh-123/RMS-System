using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RMS.Contants;
using RMS.CustomDtoValidators.Auths;
using RMS.Dtos.Auths;
using RMS.Helpers;

namespace RMS.Service.Auths;

public class RegisterUserService(
    RMSDbContext dbContext,
    IMapper mapper,
    UserManager<IdentityUser> userManager,
    ILogger<RegisterUserService> logger,
    RegisterDtoValidator validator,
    JwtTokenService jwtTokenService
) : BaseService(dbContext, mapper)
{
    private readonly ILogger<RegisterUserService> _logger = logger;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RegisterDtoValidator _validator = validator;
    private readonly JwtTokenService _jwtTokenService = jwtTokenService;
    public async Task<(bool, string, AuthResponseDto?)> ExcuteAsync(RegisterDto request)
    {
        // step 1: validate request
        var validateRequest = await _validator.ValidateAsync(request);
        if (!validateRequest.IsValid)
        {
            var errors = string.Join("/n", validateRequest.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            _logger.LogWarning("Validation failed with error: {errors}", string.Join("/n", errors));
            return (false, errors, null);
        }

        // step 2: check duplicate username
        var isDuplicatedUsername = await CheckDuplicateUsername(request.Email);
        if (isDuplicatedUsername)
        {
            _logger.LogWarning("Username already existed: {username}", request.Email);
            return (false, "Username already existed", null);
        }

        // step 2: Validate strong password
        var isStrongPassword = request.Password.IsStrongPassword();
        if (!isStrongPassword)
        {
            _logger.LogWarning("Password is not strong enough, please try a strong password");
            return (false, "Password is not strong enough, please try a strong password.", null);
        }

        // step 3: build entity
        var user = BuildEntity(request);

        // step 4: persist to database
        return await PersistAndGeneratePairToken(user, request.Password);
    }
    private static IdentityUser BuildEntity(RegisterDto request)
    {
        var user = new IdentityUser
        {
            Email = request.Email,
            UserName = request.Email,
        };

        return user;
    }
    private async Task<bool> CheckDuplicateUsername(string userName)
    {
        var normalizedName = _userManager.NormalizeName(userName);
        var user = await _userManager.FindByNameAsync(normalizedName);
        return user != null;
    }
    private async Task<(bool, string, AuthResponseDto?)> PersistAndGeneratePairToken(IdentityUser user, string password)
    {
        try
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ",result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                _logger.LogWarning("User register failed, Error: {error}", errors);
                return (false, errors, null);
            }

            var response = await _jwtTokenService.GenerateTokenPairAsync(user);

            return (true, "Registed user successfully", response);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error occurred while creating a new User");
            return (false, "Database failed operations", null);
        }
    }
}