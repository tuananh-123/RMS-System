using System.ComponentModel.DataAnnotations;

namespace RMS.Dtos.Auths;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}