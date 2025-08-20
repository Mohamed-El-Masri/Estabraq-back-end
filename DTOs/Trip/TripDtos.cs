using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.DTOs.Trip;

public class TripDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Duration { get; set; }
    public string Location { get; set; } = string.Empty;
    public string LocationAr { get; set; } = string.Empty;
    public string? MainImage { get; set; }
    public int? MaxParticipants { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public List<TripImageDto> Images { get; set; } = new();
    public List<TripScheduleDto> Schedule { get; set; } = new();
    public List<TripIncludedDto> IncludedItems { get; set; } = new();
    public int BookingsCount { get; set; }
}

public class TripSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Duration { get; set; }
    public string Location { get; set; } = string.Empty;
    public string LocationAr { get; set; } = string.Empty;
    public string? MainImage { get; set; }
    public bool IsFeatured { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
}

public class CreateTripRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic title is required")]
    [MaxLength(255, ErrorMessage = "Arabic title cannot exceed 255 characters")]
    public string TitleAr { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Arabic description cannot exceed 1000 characters")]
    public string? DescriptionAr { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Discount price must be greater than 0")]
    public decimal? DiscountPrice { get; set; }
    
    [Required(ErrorMessage = "Duration is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day")]
    public int Duration { get; set; }
    
    [Required(ErrorMessage = "Location is required")]
    [MaxLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
    public string Location { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic location is required")]
    [MaxLength(255, ErrorMessage = "Arabic location cannot exceed 255 characters")]
    public string LocationAr { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue, ErrorMessage = "Max participants must be at least 1")]
    public int? MaxParticipants { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsFeatured { get; set; } = false;
    
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }
    
    public List<CreateTripScheduleRequestDto> Schedule { get; set; } = new();
    public List<CreateTripIncludedRequestDto> IncludedItems { get; set; } = new();
}

public class UpdateTripRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic title is required")]
    [MaxLength(255, ErrorMessage = "Arabic title cannot exceed 255 characters")]
    public string TitleAr { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Arabic description cannot exceed 1000 characters")]
    public string? DescriptionAr { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Discount price must be greater than 0")]
    public decimal? DiscountPrice { get; set; }
    
    [Required(ErrorMessage = "Duration is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 day")]
    public int Duration { get; set; }
    
    [Required(ErrorMessage = "Location is required")]
    [MaxLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
    public string Location { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic location is required")]
    [MaxLength(255, ErrorMessage = "Arabic location cannot exceed 255 characters")]
    public string LocationAr { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue, ErrorMessage = "Max participants must be at least 1")]
    public int? MaxParticipants { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsFeatured { get; set; } = false;
    
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }
}
