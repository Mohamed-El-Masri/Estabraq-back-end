using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstabraqTourismAPI.Models;

public class Trip
{
    public int Id { get; set; }
    
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
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal? DiscountPrice { get; set; }
    
    [Required]
    public int Duration { get; set; } // في أيام
    
    [Required]
    [MaxLength(255)]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string LocationAr { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? MainImage { get; set; }
    
    public int? MaxParticipants { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsFeatured { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int CategoryId { get; set; }
    
    // Navigation Properties
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<TripImage> Images { get; set; } = new List<TripImage>();
    public virtual ICollection<TripSchedule> Schedule { get; set; } = new List<TripSchedule>();
    public virtual ICollection<TripIncluded> IncludedItems { get; set; } = new List<TripIncluded>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
