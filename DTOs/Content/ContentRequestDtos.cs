using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.DTOs.Content;

// Response DTOs
public class DashboardSummaryDto
{
    public int TotalUsers { get; set; }
    public int TotalTrips { get; set; }
    public int TotalBookings { get; set; }
    public int TotalCategories { get; set; }
    public int PendingBookings { get; set; }
    public int UnreadMessages { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal TotalRevenue { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewBookingsThisMonth { get; set; }
    public double BookingGrowthRate { get; set; }
    public double RevenueGrowthRate { get; set; }
    public List<QuickStatsDto> QuickStats { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public SystemHealthDto SystemHealth { get; set; } = new();
}

public class QuickStatsDto
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public double? ChangePercentage { get; set; }
    public bool IsPositive { get; set; }
}

public class RecentActivityDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // booking, user, trip, contact
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

public class SystemHealthDto
{
    public string Status { get; set; } = string.Empty; // Healthy, Warning, Critical
    public bool DatabaseConnected { get; set; }
    public bool EmailServiceWorking { get; set; }
    public bool FileUploadWorking { get; set; }
    public long MemoryUsage { get; set; }
    public double CpuUsage { get; set; }
    public long DiskSpace { get; set; }
    public long FreeDiskSpace { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime LastChecked { get; set; }
    public List<HealthCheckItemDto> HealthChecks { get; set; } = new();
}

public class HealthCheckItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public TimeSpan Duration { get; set; }
}

public class SearchRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Query { get; set; } = string.Empty;

    public string? Type { get; set; } // trips, categories, all
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class SearchResultDto
{
    public string Query { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public List<SearchItemDto> Trips { get; set; } = new();
    public List<SearchItemDto> Categories { get; set; } = new();
    public TimeSpan SearchTime { get; set; }
}

public class SearchItemDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // trip, category
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Url { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Location { get; set; }
    public double Relevance { get; set; }
}
