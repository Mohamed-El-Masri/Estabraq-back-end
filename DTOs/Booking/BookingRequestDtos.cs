using System.ComponentModel.DataAnnotations;
using EstabraqTourismAPI.DTOs.Common;
using EstabraqTourismAPI.DTOs.Trip;
using EstabraqTourismAPI.DTOs.User;

namespace EstabraqTourismAPI.DTOs.Booking;

// Request DTOs
public class GetBookingsRequestDto : PaginationRequestDto
{
    public string? SearchTerm { get; set; }
    public string? BookingStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public int? TripId { get; set; }
    public int? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TravelStartDate { get; set; }
    public DateTime? TravelEndDate { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public new string? SortBy { get; set; } = "BookingDate";
    public string? SortDirection { get; set; } = "desc";
}

public class GetBookingsRequestDtoOld : PaginationRequestDto
{
    public string? Status { get; set; }
    public int? TripId { get; set; }
    public int? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class RevenueStatisticsRequestDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string GroupBy { get; set; } = "month"; // day, week, month, year
}

// Response DTOs
public class BookingDetailDto : BookingDto
{
    public TripDto? Trip { get; set; }
    public UserDto? User { get; set; }
    public new string? AdminNotes { get; set; }
    public List<BookingHistoryDto> History { get; set; } = new();
}

public class BookingHistoryDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class BookingStatisticsDto
{
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int CompletedBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int BookingsThisMonth { get; set; }
    public int BookingsLastMonth { get; set; }
    public double BookingGrowthRate { get; set; }
    public decimal AverageBookingValue { get; set; }
    public List<MonthlyBookingStatsDto> MonthlyStats { get; set; } = new();
    public List<TripBookingStatsDto> TopTrips { get; set; } = new();
}

public class MonthlyBookingStatsDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
    public string MonthName { get; set; } = string.Empty;
}

public class TripBookingStatsDto
{
    public int TripId { get; set; }
    public string TripTitle { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
}

public class RevenueStatisticsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalBookings { get; set; }
    public decimal AverageBookingValue { get; set; }
    public List<RevenueDataPointDto> RevenueData { get; set; } = new();
    public Dictionary<string, decimal> RevenueByStatus { get; set; } = new();
    public Dictionary<string, decimal> RevenueByTrip { get; set; } = new();
}

public class RevenueDataPointDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int BookingCount { get; set; }
    public DateTime Date { get; set; }
}
