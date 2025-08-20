using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.Models;

public class HeroSection
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string TitleAr { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Subtitle { get; set; }
    
    [MaxLength(1000)]
    public string? SubtitleAr { get; set; }
    
    [MaxLength(500)]
    public string? BackgroundImage { get; set; }
    
    [MaxLength(500)]
    public string? BackgroundVideo { get; set; }
    
    [MaxLength(255)]
    public string? ButtonText { get; set; }
    
    [MaxLength(255)]
    public string? ButtonTextAr { get; set; }
    
    [MaxLength(500)]
    public string? ButtonLink { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
