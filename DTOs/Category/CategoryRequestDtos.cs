using System.ComponentModel.DataAnnotations;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.DTOs.Category;

// Request DTOs
public class GetCategoriesRequestDto : PaginationRequestDto
{
    public bool? IsActive { get; set; }
    public new string? SortBy { get; set; } = "name"; // name, createdAt, tripsCount
    public bool IsDescending { get; set; } = false;
}

// Statistics DTOs
public class CategoryStatisticsDto
{
    public int TotalCategories { get; set; }
    public int ActiveCategories { get; set; }
    public int InactiveCategories { get; set; }
    public int CategoriesWithTrips { get; set; }
    public int EmptyCategories { get; set; }
    public CategoryTripStatsDto MostPopular { get; set; } = new();
    public List<CategoryTripStatsDto> CategoryTripCounts { get; set; } = new();
    public DateTime LastCategoryAdded { get; set; }
}

public class CategoryTripStatsDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public int TripCount { get; set; }
    public int ActiveTripCount { get; set; }
    public decimal AverageTripPrice { get; set; }
}
