using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.Models;

public class TripIncluded
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Item { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string ItemAr { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Icon { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int TripId { get; set; }
    
    // Navigation Properties
    public virtual Trip Trip { get; set; } = null!;
}
