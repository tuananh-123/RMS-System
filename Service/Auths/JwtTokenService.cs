using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RMS.Dtos.Auths;
using RMS.Entities;

namespace RMS.Service.Auths;

public class JwtTokenService(
    IConfiguration configuration,
    RMSDbContext dbContext
)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly RMSDbContext _dbContext = dbContext;

    public async Task<AuthResponseDto> GenerateTokenPairAsync(IdentityUser user)
    {
        var accessToken = GenerateAccessToken(user, out var accessExpiresAt);
        var (refreshToken, refreshExpiresAt) = await GenerateAndSaveRefreshTokenAsync(user.Id);

        return new AuthResponseDto
        {
            Email = user.Email!,
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiresAt  
        };
    }

    public async Task<(string Token, DateTime ExpiresAt)> GenerateAndSaveRefreshTokenAsync(string userId)
    {
        var jwtSetting = _configuration.GetSection("Jwt");
        var expiresAt = DateTime.UtcNow.AddDays(double.Parse(jwtSetting["RefreshTokenExpiredTime"]!));
        // tao refresh token ngau nhien 64 bytes
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var expiredTokens = _dbContext.RefreshTokens
            .Where(rt => rt.UserID == userId && (rt.IsRevoked || rt.ExpiresAt < DateTime.UtcNow));
        
        _dbContext.RefreshTokens.RemoveRange(expiredTokens);

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserID = userId
        });

        await _dbContext.SaveChangesAsync();
        return (token, expiresAt);
    }

    public string GenerateAccessToken(IdentityUser user, out DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-min-32-chars!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:AccessTokenExpiredTime"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "RMS",
            audience: _configuration["Jwt:Audience"] ?? "RMS API",
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }

}