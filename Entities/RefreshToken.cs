using Microsoft.AspNetCore.Identity;

namespace RMS.Entities;

public class RefreshToken
{
    public int ID { get; set; }
    public string Token { get; set; } = string.Empty;
    public string UserID { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;

    public IdentityUser User { get; set; } = null!;
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}