using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TripsCount { get; set; }
}

public class CreateCategoryRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic name is required")]
    [MaxLength(255, ErrorMessage = "Arabic name cannot exceed 255 characters")]
    public string NameAr { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Arabic description cannot exceed 1000 characters")]
    public string? DescriptionAr { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public class UpdateCategoryRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic name is required")]
    [MaxLength(255, ErrorMessage = "Arabic name cannot exceed 255 characters")]
    public string NameAr { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Arabic description cannot exceed 1000 characters")]
    public string? DescriptionAr { get; set; }
    
    public bool IsActive { get; set; } = true;
}
