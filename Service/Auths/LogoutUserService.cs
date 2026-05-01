namespace RMS.Service.Auths;

public class LogoutUserService(
    RMSDbContext dbContext
)
{
    private readonly RMSDbContext _dbContext = dbContext;
    public async Task<(bool, string)> ExecuteAsync(string refreshToken)
    {
        var storedToken = _dbContext.RefreshTokens
            .FirstOrDefault(rt => rt.Token == refreshToken);

        if (storedToken == null || !storedToken.IsActive)
            return (false, "RefreshToken không hợp lệ hoặc đã hết hạn");
        
        storedToken.IsRevoked = true;
        await _dbContext.SaveChangesAsync();

        return (true, "Logout successfully");
    }
}