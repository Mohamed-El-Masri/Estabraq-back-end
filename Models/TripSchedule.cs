using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.Models;

public class TripSchedule
{
    public int Id { get; set; }
    
    public int DayNumber { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string TitleAr { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(1000)]
    public string? DescriptionAr { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int TripId { get; set; }
    
    // Navigation Properties
    public virtual Trip Trip { get; set; } = null!;
}
