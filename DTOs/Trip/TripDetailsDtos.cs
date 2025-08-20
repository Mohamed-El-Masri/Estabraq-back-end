using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.DTOs.Trip;

public class TripImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? CaptionAr { get; set; }
    public int SortOrder { get; set; }
}

public class CreateTripImageRequestDto
{
    [MaxLength(255, ErrorMessage = "Caption cannot exceed 255 characters")]
    public string? Caption { get; set; }
    
    [MaxLength(255, ErrorMessage = "Arabic caption cannot exceed 255 characters")]
    public string? CaptionAr { get; set; }
    
    public int SortOrder { get; set; } = 0;
}

public class TripScheduleDto
{
    public int Id { get; set; }
    public int DayNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}

public class CreateTripScheduleRequestDto
{
    [Required(ErrorMessage = "Day number is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Day number must be at least 1")]
    public int DayNumber { get; set; }
    
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
    
    public int SortOrder { get; set; } = 0;
}

public class TripIncludedDto
{
    public int Id { get; set; }
    public string Item { get; set; } = string.Empty;
    public string ItemAr { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
}

public class CreateTripIncludedRequestDto
{
    [Required(ErrorMessage = "Item is required")]
    [MaxLength(255, ErrorMessage = "Item cannot exceed 255 characters")]
    public string Item { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic item is required")]
    [MaxLength(255, ErrorMessage = "Arabic item cannot exceed 255 characters")]
    public string ItemAr { get; set; } = string.Empty;
    
    public int SortOrder { get; set; } = 0;
}
