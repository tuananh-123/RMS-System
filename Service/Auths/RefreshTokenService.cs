using Microsoft.EntityFrameworkCore;
using RMS.Dtos.Auths;

namespace RMS.Service.Auths;

public class RefreshTokenService(
    RMSDbContext dbContext,
    JwtTokenService jwtTokenService
)
{
    private readonly RMSDbContext _dbContext = dbContext;
    private readonly JwtTokenService _jwtTokenService = jwtTokenService;

    public async Task<(bool, string, AuthResponseDto?)> ExecuteAsync(string refreshToken)
    {
        var storedToken = _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefault(rt => rt.Token == refreshToken);

        if (storedToken == null)
            return (false, "RefreshToken is not valid", null);
        
        if (!storedToken.IsActive)
        {
            var reason = storedToken.IsRevoked ? "Token has been revoked": "Token has expired";
            return (false, reason, null);
        }

        // thu hồi token cũ
        storedToken.IsRevoked = true;
        await _dbContext.SaveChangesAsync();

        var response = await _jwtTokenService.GenerateTokenPairAsync(storedToken.User);
        return (true, "Refresh token successfully", response);
        
    }
}