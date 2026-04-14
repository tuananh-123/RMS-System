using System.ComponentModel.DataAnnotations;

namespace RMS.Entities;

public class BaseEntity
{
    public int ID { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public string CreatedBy { get; set; } = string.Empty;
    // Soft delete
    public bool Trash { get; set; } = false;
}
