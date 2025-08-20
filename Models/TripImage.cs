using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.Models;

public class TripImage
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? Caption { get; set; }
    
    [MaxLength(255)]
    public string? CaptionAr { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int TripId { get; set; }
    
    // Navigation Properties
    public virtual Trip Trip { get; set; } = null!;
}
