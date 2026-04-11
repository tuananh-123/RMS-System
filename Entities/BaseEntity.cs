using System.ComponentModel.DataAnnotations;

namespace RMS.Entities;

public class BaseEntity
{
    public int ID { get; set; }
    [Required]
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public string CreatedBy { get; set; }
    // Soft delete
    public bool Trash { get; set; } = false;
}
