using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.Models;

public class ContactMessage
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Subject { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Status { get; set; } = "New"; // New, InProgress, Resolved
    
    [MaxLength(2000)]
    public string? AdminReply { get; set; }
    
    public DateTime? RepliedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int? RepliedByUserId { get; set; }
    
    // Navigation Properties
    public virtual User? RepliedByUser { get; set; }
}
