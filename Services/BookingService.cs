using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EstabraqTourismAPI.Data;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Booking;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Services;

public interface IBookingService
{
    Task<ApiResponse<PagedResult<BookingDto>>> GetBookingsAsync(PaginationRequestDto parameters);
    Task<ApiResponse<PagedResult<BookingDto>>> GetUserBookingsAsync(int userId, PaginationRequestDto parameters);
    Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id);
    Task<ApiResponse<BookingDetailDto>> GetBookingByIdAsync(int id, int userId, bool isAdmin);
    Task<ApiResponse<BookingDto>> GetBookingByReferenceAsync(string reference);
    Task<ApiResponse<BookingDto>> CreateBookingAsync(CreateBookingRequestDto request, int? userId = null);
    Task<ApiResponse<BookingDto>> UpdateBookingStatusAsync(int id, UpdateBookingStatusRequestDto request, int adminUserId);
    Task<ApiResponse<string>> CancelBookingAsync(int id, int? userId = null);
    Task<ApiResponse<string>> CancelBookingAsync(int id, int userId, string reason);
    Task<ApiResponse<string>> DeleteBookingAsync(int id);
    Task<ApiResponse<BookingStatsDto>> GetBookingStatsAsync();
    Task<ApiResponse<object>> GetBookingStatisticsAsync();
    Task<ApiResponse<object>> GetRevenueStatisticsAsync();
    Task<ApiResponse<List<BookingDto>>> GetRecentBookingsAsync(int count = 10);
}

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<BookingService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<BookingDto>>> GetBookingsAsync(PaginationRequestDto parameters)
    {
        try
        {
            var query = _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.User)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(b => b.BookingReference.Contains(parameters.Search) ||
                                        b.CustomerName.Contains(parameters.Search) ||
                                        b.CustomerEmail.Contains(parameters.Search) ||
                                        b.CustomerPhone.Contains(parameters.Search) ||
                                        b.Trip.Title.Contains(parameters.Search));
            }

            // Sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "reference":
                        query = parameters.SortDescending ? query.OrderByDescending(b => b.BookingReference) : query.OrderBy(b => b.BookingReference);
                        break;
                    case "customer":
                        query = parameters.SortDescending ? query.OrderByDescending(b => b.CustomerName) : query.OrderBy(b => b.CustomerName);
                        break;
                    case "status":
                        query = parameters.SortDescending ? query.OrderByDescending(b => b.Status) : query.OrderBy(b => b.Status);
                        break;
                    case "bookingdate":
                        query = parameters.SortDescending ? query.OrderByDescending(b => b.BookingDate) : query.OrderBy(b => b.BookingDate);
                        break;
                    case "totalprice":
                        query = parameters.SortDescending ? query.OrderByDescending(b => b.TotalPrice) : query.OrderBy(b => b.TotalPrice);
                        break;
                    default:
                        query = query.OrderByDescending(b => b.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(b => b.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var bookings = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);

            var result = new PagedResult<BookingDto>
            {
                Items = bookingDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            return ApiResponse<PagedResult<BookingDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings");
            return ApiResponse<PagedResult<BookingDto>>.FailureResult("An error occurred while getting bookings");
        }
    }

    public async Task<ApiResponse<PagedResult<BookingDto>>> GetUserBookingsAsync(int userId, PaginationRequestDto parameters)
    {
        try
        {
            var query = _context.Bookings
                .Include(b => b.Trip)
                .Where(b => b.UserId == userId)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(b => b.BookingReference.Contains(parameters.Search) ||
                                        b.Trip.Title.Contains(parameters.Search) ||
                                        b.Status.Contains(parameters.Search));
            }

            // Sorting
            query = query.OrderByDescending(b => b.CreatedAt);

            var totalCount = await query.CountAsync();
            var bookings = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);

            var result = new PagedResult<BookingDto>
            {
                Items = bookingDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            return ApiResponse<PagedResult<BookingDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user bookings for user {UserId}", userId);
            return ApiResponse<PagedResult<BookingDto>>.FailureResult("An error occurred while getting user bookings");
        }
    }

    public async Task<ApiResponse<BookingDto>> GetBookingByIdAsync(int id)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return ApiResponse<BookingDto>.FailureResult("Booking not found");
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);
            return ApiResponse<BookingDto>.SuccessResult(bookingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking {BookingId}", id);
            return ApiResponse<BookingDto>.FailureResult("An error occurred while getting booking");
        }
    }

    public async Task<ApiResponse<BookingDto>> GetBookingByReferenceAsync(string reference)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingReference == reference);

            if (booking == null)
            {
                return ApiResponse<BookingDto>.FailureResult("Booking not found");
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);
            return ApiResponse<BookingDto>.SuccessResult(bookingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking by reference {Reference}", reference);
            return ApiResponse<BookingDto>.FailureResult("An error occurred while getting booking");
        }
    }

    public async Task<ApiResponse<BookingDto>> CreateBookingAsync(CreateBookingRequestDto request, int? userId = null)
    {
        try
        {
            var trip = await _context.Trips.FindAsync(request.TripId);
            if (trip == null)
            {
                return ApiResponse<BookingDto>.FailureResult("Trip not found");
            }

            if (!trip.IsActive)
            {
                return ApiResponse<BookingDto>.FailureResult("Trip is not available for booking");
            }

            // Check availability if max participants is set
            if (trip.MaxParticipants.HasValue)
            {
                var existingBookings = await _context.Bookings
                    .Where(b => b.TripId == request.TripId && 
                               (b.Status == "Confirmed" || b.Status == "Pending"))
                    .SumAsync(b => b.NumberOfPeople);

                if (existingBookings + request.NumberOfPeople > trip.MaxParticipants.Value)
                {
                    return ApiResponse<BookingDto>.FailureResult("Not enough spaces available for this trip");
                }
            }

            var booking = _mapper.Map<Booking>(request);
            booking.BookingReference = GenerateBookingReference();
            booking.TotalPrice = CalculateTotalPrice(trip, request.NumberOfPeople);
            booking.UserId = userId;
            booking.BookingDate = DateTime.UtcNow;
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Load related data for response
            await _context.Entry(booking)
                .Reference(b => b.Trip)
                .LoadAsync();

            if (userId.HasValue)
            {
                await _context.Entry(booking)
                    .Reference(b => b.User)
                    .LoadAsync();
            }

            var bookingDto = _mapper.Map<BookingDto>(booking);
            _logger.LogInformation("Booking created with reference {BookingReference}", booking.BookingReference);
            return ApiResponse<BookingDto>.SuccessResult(bookingDto, "Booking created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return ApiResponse<BookingDto>.FailureResult("An error occurred while creating booking");
        }
    }

    public async Task<ApiResponse<BookingDto>> UpdateBookingStatusAsync(int id, UpdateBookingStatusRequestDto request, int adminUserId)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return ApiResponse<BookingDto>.FailureResult("Booking not found");
            }

            booking.Status = request.Status;
            booking.AdminNotes = request.AdminNotes;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var bookingDto = _mapper.Map<BookingDto>(booking);
            _logger.LogInformation("Booking {BookingId} status updated to {Status} by admin {AdminId}", id, request.Status, adminUserId);
            return ApiResponse<BookingDto>.SuccessResult(bookingDto, "Booking status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking status {BookingId}", id);
            return ApiResponse<BookingDto>.FailureResult("An error occurred while updating booking status");
        }
    }

    public async Task<ApiResponse<string>> CancelBookingAsync(int id, int? userId = null)
    {
        try
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return ApiResponse<string>.FailureResult("Booking not found");
            }

            // If userId is provided, ensure the booking belongs to that user
            if (userId.HasValue && booking.UserId != userId.Value)
            {
                return ApiResponse<string>.FailureResult("You can only cancel your own bookings");
            }

            if (booking.Status == "Cancelled")
            {
                return ApiResponse<string>.FailureResult("Booking is already cancelled");
            }

            if (booking.Status == "Completed")
            {
                return ApiResponse<string>.FailureResult("Cannot cancel completed booking");
            }

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Booking {BookingId} cancelled", id);
            return ApiResponse<string>.SuccessResult("", "Booking cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while cancelling booking");
        }
    }

    public async Task<ApiResponse<BookingStatsDto>> GetBookingStatsAsync()
    {
        try
        {
            var stats = new BookingStatsDto
            {
                TotalBookings = await _context.Bookings.CountAsync(),
                PendingBookings = await _context.Bookings.CountAsync(b => b.Status == "Pending"),
                ConfirmedBookings = await _context.Bookings.CountAsync(b => b.Status == "Confirmed"),
                CancelledBookings = await _context.Bookings.CountAsync(b => b.Status == "Cancelled"),
                CompletedBookings = await _context.Bookings.CountAsync(b => b.Status == "Completed"),
                TotalRevenue = await _context.Bookings
                    .Where(b => b.Status == "Confirmed" || b.Status == "Completed")
                    .SumAsync(b => b.TotalPrice),
                PendingRevenue = await _context.Bookings
                    .Where(b => b.Status == "Pending")
                    .SumAsync(b => b.TotalPrice),
                ConfirmedRevenue = await _context.Bookings
                    .Where(b => b.Status == "Confirmed")
                    .SumAsync(b => b.TotalPrice)
            };

            return ApiResponse<BookingStatsDto>.SuccessResult(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking stats");
            return ApiResponse<BookingStatsDto>.FailureResult("An error occurred while getting booking stats");
        }
    }

    public async Task<ApiResponse<List<BookingDto>>> GetRecentBookingsAsync(int count = 10)
    {
        try
        {
            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .Take(count)
                .ToListAsync();

            var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
            return ApiResponse<List<BookingDto>>.SuccessResult(bookingDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent bookings");
            return ApiResponse<List<BookingDto>>.FailureResult("An error occurred while getting recent bookings");
        }
    }

    private string GenerateBookingReference()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"BK{timestamp}{random}";
    }

    private decimal CalculateTotalPrice(Trip trip, int numberOfPeople)
    {
        var price = trip.DiscountPrice ?? trip.Price;
        return price * numberOfPeople;
    }

    public async Task<ApiResponse<BookingDetailDto>> GetBookingByIdAsync(int id, int userId, bool isAdmin)
    {
        try
        {
            var query = _context.Bookings
                .Include(b => b.Trip)
                .Include(b => b.User)
                .AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(b => b.UserId == userId);
            }

            var booking = await query.FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return ApiResponse<BookingDetailDto>.FailureResult("Booking not found");
            }

            var bookingDetail = _mapper.Map<BookingDetailDto>(booking);
            return ApiResponse<BookingDetailDto>.SuccessResult(bookingDetail, "Booking retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking {BookingId}", id);
            return ApiResponse<BookingDetailDto>.FailureResult("An error occurred while retrieving booking");
        }
    }

    public async Task<ApiResponse<string>> CancelBookingAsync(int id, int userId, string reason)
    {
        try
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return ApiResponse<string>.FailureResult("Booking not found");
            }

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ApiResponse<string>.SuccessResult("", "Booking cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while cancelling booking");
        }
    }

    public async Task<ApiResponse<string>> DeleteBookingAsync(int id)
    {
        try
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return ApiResponse<string>.FailureResult("Booking not found");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.SuccessResult("", "Booking deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking {BookingId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while deleting booking");
        }
    }

    public async Task<ApiResponse<object>> GetBookingStatisticsAsync()
    {
        try
        {
            var totalBookings = await _context.Bookings.CountAsync();
            var confirmedBookings = await _context.Bookings.CountAsync(b => b.Status == "Confirmed");
            var pendingBookings = await _context.Bookings.CountAsync(b => b.Status == "Pending");
            var cancelledBookings = await _context.Bookings.CountAsync(b => b.Status == "Cancelled");
            var totalRevenue = await _context.Bookings
                .Where(b => b.Status == "Confirmed")
                .SumAsync(b => b.TotalPrice);

            var stats = new
            {
                TotalBookings = totalBookings,
                ConfirmedBookings = confirmedBookings,
                PendingBookings = pendingBookings,
                CancelledBookings = cancelledBookings,
                TotalRevenue = totalRevenue
            };

            return ApiResponse<object>.SuccessResult(stats, "Booking statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking statistics");
            return ApiResponse<object>.FailureResult("An error occurred while retrieving booking statistics");
        }
    }

    public async Task<ApiResponse<object>> GetRevenueStatisticsAsync()
    {
        try
        {
            var thisMonth = await _context.Bookings
                .Where(b => b.Status == "Confirmed" && b.CreatedAt.Month == DateTime.UtcNow.Month)
                .SumAsync(b => b.TotalPrice);

            var lastMonth = await _context.Bookings
                .Where(b => b.Status == "Confirmed" && b.CreatedAt.Month == DateTime.UtcNow.AddMonths(-1).Month)
                .SumAsync(b => b.TotalPrice);

            var stats = new
            {
                ThisMonth = thisMonth,
                LastMonth = lastMonth,
                GrowthPercentage = lastMonth > 0 ? ((thisMonth - lastMonth) / lastMonth) * 100 : 0
            };

            return ApiResponse<object>.SuccessResult(stats, "Revenue statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue statistics");
            return ApiResponse<object>.FailureResult("An error occurred while retrieving revenue statistics");
        }
    }
}
