using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.Models;

public class SiteStats
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string TitleAr { get; set; } = string.Empty;
    
    [Required]
    public int Value { get; set; }
    
    [MaxLength(500)]
    public string? Icon { get; set; }
    
    [MaxLength(255)]
    public string? Color { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
