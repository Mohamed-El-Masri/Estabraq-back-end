using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstabraqTourismAPI.Models;

public class Booking
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string BookingReference { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string CustomerPhone { get; set; } = string.Empty;
    
    [Required]
    public int NumberOfPeople { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
    
    [MaxLength(1000)]
    public string? SpecialRequests { get; set; }
    
    [MaxLength(1000)]
    public string? AdminNotes { get; set; }
    
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int TripId { get; set; }
    public int? UserId { get; set; } // nullable للحجوزات من الضيوف
    
    // Navigation Properties
    public virtual Trip Trip { get; set; } = null!;
    public virtual User? User { get; set; }
}
