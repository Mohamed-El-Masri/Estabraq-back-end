using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.DTOs.Content;

public class HeroSectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? SubtitleAr { get; set; }
    public string? BackgroundImage { get; set; }
    public string? BackgroundVideo { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonTextAr { get; set; }
    public string? ButtonLink { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateHeroSectionRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic title is required")]
    [MaxLength(255, ErrorMessage = "Arabic title cannot exceed 255 characters")]
    public string TitleAr { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Subtitle cannot exceed 1000 characters")]
    public string? Subtitle { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Arabic subtitle cannot exceed 1000 characters")]
    public string? SubtitleAr { get; set; }
    
    [MaxLength(255, ErrorMessage = "Button text cannot exceed 255 characters")]
    public string? ButtonText { get; set; }
    
    [MaxLength(255, ErrorMessage = "Arabic button text cannot exceed 255 characters")]
    public string? ButtonTextAr { get; set; }
    
    [MaxLength(500, ErrorMessage = "Button link cannot exceed 500 characters")]
    public string? ButtonLink { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
}

public class UpdateHeroSectionRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic title is required")]
    [MaxLength(255, ErrorMessage = "Arabic title cannot exceed 255 characters")]
    public string TitleAr { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "Subtitle cannot exceed 1000 characters")]
    public string? Subtitle { get; set; }
    
    [MaxLength(1000, ErrorMessage = "Arabic subtitle cannot exceed 1000 characters")]
    public string? SubtitleAr { get; set; }
    
    [MaxLength(255, ErrorMessage = "Button text cannot exceed 255 characters")]
    public string? ButtonText { get; set; }
    
    [MaxLength(255, ErrorMessage = "Arabic button text cannot exceed 255 characters")]
    public string? ButtonTextAr { get; set; }
    
    [MaxLength(500, ErrorMessage = "Button link cannot exceed 500 characters")]
    public string? ButtonLink { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
}

public class SiteStatsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public int Value { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateSiteStatsRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic title is required")]
    [MaxLength(255, ErrorMessage = "Arabic title cannot exceed 255 characters")]
    public string TitleAr { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Value is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Value must be non-negative")]
    public int Value { get; set; }
    
    [MaxLength(255, ErrorMessage = "Color cannot exceed 255 characters")]
    public string? Color { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
}

public class UpdateSiteStatsRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Arabic title is required")]
    [MaxLength(255, ErrorMessage = "Arabic title cannot exceed 255 characters")]
    public string TitleAr { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Value is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Value must be non-negative")]
    public int Value { get; set; }
    
    [MaxLength(255, ErrorMessage = "Color cannot exceed 255 characters")]
    public string? Color { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int SortOrder { get; set; } = 0;
}
