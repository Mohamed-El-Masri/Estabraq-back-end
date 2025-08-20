using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.Models;

public class ContactInfo
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string CompanyNameAr { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(500)]
    public string? AddressAr { get; set; }
    
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    [MaxLength(20)]
    public string? WhatsApp { get; set; }
    
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }
    
    [MaxLength(500)]
    public string? Website { get; set; }
    
    [MaxLength(500)]
    public string? Facebook { get; set; }
    
    [MaxLength(500)]
    public string? Instagram { get; set; }
    
    [MaxLength(500)]
    public string? Twitter { get; set; }
    
    [MaxLength(500)]
    public string? YouTube { get; set; }
    
    [MaxLength(500)]
    public string? TikTok { get; set; }
    
    [MaxLength(1000)]
    public string? WorkingHours { get; set; }
    
    [MaxLength(1000)]
    public string? WorkingHoursAr { get; set; }
    
    [MaxLength(500)]
    public string? Logo { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
