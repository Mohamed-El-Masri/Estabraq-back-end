using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstabraqTourismAPI.Services;
using EstabraqTourismAPI.DTOs.Booking;
using EstabraqTourismAPI.DTOs.Common;
using EstabraqTourismAPI.Helpers;

namespace EstabraqTourismAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Get all bookings (Admin only)
    /// </summary>
    /// <param name="request">Pagination, search, and filter parameters</param>
    /// <returns>Paginated list of bookings</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BookingDto>>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<PagedResult<BookingDto>>>> GetBookings([FromQuery] GetBookingsRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<PagedResult<BookingDto>>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _bookingService.GetBookingsAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetBookings endpoint");
            return StatusCode(500, ApiResponse<PagedResult<BookingDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get user's bookings
    /// </summary>
    /// <param name="request">Pagination parameters</param>
    /// <returns>Paginated list of user's bookings</returns>
    [HttpGet("my-bookings")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BookingDto>>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<PagedResult<BookingDto>>>> GetMyBookings([FromQuery] PaginationRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<PagedResult<BookingDto>>.FailureResult(
                    "Validation failed", errors));
            }

            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<PagedResult<BookingDto>>.FailureResult("User not authenticated"));
            }

            var result = await _bookingService.GetUserBookingsAsync(userId.Value, request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMyBookings endpoint");
            return StatusCode(500, ApiResponse<PagedResult<BookingDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get booking by ID
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Booking information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDetailDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BookingDetailDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<BookingDetailDto>>> GetBooking(int id)
    {
        try
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();
            
            if (userId == null)
            {
                return Unauthorized(ApiResponse<BookingDetailDto>.FailureResult("User not authenticated"));
            }

            var result = await _bookingService.GetBookingByIdAsync(id, userId.Value, userRole == "Admin");
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetBooking endpoint for ID {BookingId}", id);
            return StatusCode(500, ApiResponse<BookingDetailDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Create new booking
    /// </summary>
    /// <param name="request">Booking creation data</param>
    /// <returns>Created booking information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookingDetailDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<BookingDetailDto>), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<BookingDetailDto>>> CreateBooking([FromBody] CreateBookingRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<BookingDetailDto>.FailureResult(
                    "Validation failed", errors));
            }

            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<BookingDetailDto>.FailureResult("User not authenticated"));
            }

            var result = await _bookingService.CreateBookingAsync(request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetBooking), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateBooking endpoint");
            return StatusCode(500, ApiResponse<BookingDetailDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Update booking status (Admin only)
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Status update data</param>
    /// <returns>Updated booking information</returns>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<BookingDto>.FailureResult(
                    "Validation failed", errors));
            }

            var adminUserId = User.GetUserId();
            if (adminUserId == null)
            {
                return Unauthorized(ApiResponse<BookingDto>.FailureResult("User not authenticated"));
            }

            var result = await _bookingService.UpdateBookingStatusAsync(id, request, adminUserId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateBookingStatus endpoint for ID {BookingId}", id);
            return StatusCode(500, ApiResponse<BookingDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Cancel booking
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Updated booking information</returns>
    [HttpPatch("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CancelBooking(int id)
    {
        try
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();
            
            if (userId == null)
            {
                return Unauthorized(ApiResponse<BookingDto>.FailureResult("User not authenticated"));
            }

            var result = await _bookingService.CancelBookingAsync(id, userId.Value, "User requested cancellation");
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CancelBooking endpoint for ID {BookingId}", id);
            return StatusCode(500, ApiResponse<BookingDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Delete booking (Admin only)
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<string>>> DeleteBooking(int id)
    {
        try
        {
            var result = await _bookingService.DeleteBookingAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteBooking endpoint for ID {BookingId}", id);
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get booking statistics (Admin only)
    /// </summary>
    /// <returns>Booking statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<BookingStatisticsDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<BookingStatisticsDto>>> GetBookingStatistics()
    {
        try
        {
            var result = await _bookingService.GetBookingStatisticsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetBookingStatistics endpoint");
            return StatusCode(500, ApiResponse<BookingStatisticsDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get revenue statistics (Admin only)
    /// </summary>
    /// <param name="request">Date range and grouping parameters</param>
    /// <returns>Revenue statistics</returns>
    [HttpGet("revenue-statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<RevenueStatisticsDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<RevenueStatisticsDto>>> GetRevenueStatistics([FromQuery] RevenueStatisticsRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<RevenueStatisticsDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _bookingService.GetRevenueStatisticsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetRevenueStatistics endpoint");
            return StatusCode(500, ApiResponse<RevenueStatisticsDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }
}
