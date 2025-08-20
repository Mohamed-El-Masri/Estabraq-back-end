using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int NumberOfPeople { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TripId { get; set; }
    public string TripTitle { get; set; } = string.Empty;
    public string TripTitleAr { get; set; } = string.Empty;
    public string TripLocation { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string? UserName { get; set; }
}

public class CreateBookingRequestDto
{
    [Required(ErrorMessage = "Customer name is required")]
    [MaxLength(255, ErrorMessage = "Customer name cannot exceed 255 characters")]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Customer email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Customer phone is required")]
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string CustomerPhone { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Number of people is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of people must be at least 1")]
    public int NumberOfPeople { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Special requests cannot exceed 1000 characters")]
    public string? SpecialRequests { get; set; }
    
    [Required(ErrorMessage = "Trip ID is required")]
    public int TripId { get; set; }
}

public class UpdateBookingStatusRequestDto
{
    [Required(ErrorMessage = "Status is required")]
    [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string Status { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Admin notes cannot exceed 1000 characters")]
    public string? AdminNotes { get; set; }
}

public class BookingStatsDto
{
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int CompletedBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal PendingRevenue { get; set; }
    public decimal ConfirmedRevenue { get; set; }
}
