using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstabraqTourismAPI.Services;
using EstabraqTourismAPI.DTOs.Trip;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;
    private readonly ILogger<TripsController> _logger;

    public TripsController(ITripService tripService, ILogger<TripsController> logger)
    {
        _tripService = tripService;
        _logger = logger;
    }

    /// <summary>
    /// Get all trips (Public)
    /// </summary>
    /// <param name="request">Pagination, search, and filter parameters</param>
    /// <returns>Paginated list of trips</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TripDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<TripDto>>>> GetTrips([FromQuery] PaginationRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<PagedResult<TripDto>>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _tripService.GetTripsAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrips endpoint");
            return StatusCode(500, ApiResponse<PagedResult<TripDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get trip by ID (Public)
    /// </summary>
    /// <param name="id">Trip ID</param>
    /// <returns>Trip information with details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 404)]
    public async Task<ActionResult<ApiResponse<TripDto>>> GetTrip(int id)
    {
        try
        {
            var result = await _tripService.GetTripByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTrip endpoint for ID {TripId}", id);
            return StatusCode(500, ApiResponse<TripDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get trip by slug (Public)
    /// </summary>
    /// <param name="slug">Trip slug</param>
    /// <returns>Trip information with details</returns>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 404)]
    public async Task<ActionResult<ApiResponse<TripDto>>> GetTripBySlug(string slug)
    {
        try
        {
            var result = await _tripService.GetTripBySlugAsync(slug);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTripBySlug endpoint for slug {TripSlug}", slug);
            return StatusCode(500, ApiResponse<TripDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get featured trips (Public)
    /// </summary>
    /// <param name="count">Number of featured trips to return</param>
    /// <returns>List of featured trips</returns>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(ApiResponse<List<TripDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<TripDto>>>> GetFeaturedTrips([FromQuery] int count = 6)
    {
        try
        {
            var result = await _tripService.GetFeaturedTripsAsync(count);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetFeaturedTrips endpoint");
            return StatusCode(500, ApiResponse<List<TripDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get trips by category (Public)
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="request">Pagination parameters</param>
    /// <returns>Paginated list of trips in category</returns>
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TripDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TripDto>>), 404)]
    public async Task<ActionResult<ApiResponse<PagedResult<TripDto>>>> GetTripsByCategory(
        int categoryId, 
        [FromQuery] PaginationRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<PagedResult<TripDto>>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _tripService.GetTripsByCategoryAsync(categoryId, 10);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTripsByCategory endpoint for category {CategoryId}", categoryId);
            return StatusCode(500, ApiResponse<PagedResult<TripDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Create new trip (Admin only)
    /// </summary>
    /// <param name="request">Trip creation data</param>
    /// <returns>Created trip information</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<TripDto>>> CreateTrip([FromBody] CreateTripRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<TripDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _tripService.CreateTripAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetTrip), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateTrip endpoint");
            return StatusCode(500, ApiResponse<TripDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Update trip (Admin only)
    /// </summary>
    /// <param name="id">Trip ID</param>
    /// <param name="request">Updated trip data</param>
    /// <returns>Updated trip information</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<TripDto>>> UpdateTrip(int id, [FromBody] UpdateTripRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<TripDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _tripService.UpdateTripAsync(id, request);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateTrip endpoint for ID {TripId}", id);
            return StatusCode(500, ApiResponse<TripDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Delete trip (Admin only)
    /// </summary>
    /// <param name="id">Trip ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<string>>> DeleteTrip(int id)
    {
        try
        {
            var result = await _tripService.DeleteTripAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteTrip endpoint for ID {TripId}", id);
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Toggle trip active status (Admin only)
    /// </summary>
    /// <param name="id">Trip ID</param>
    /// <returns>Updated trip information</returns>
    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<TripDto>>> ToggleTripStatus(int id)
    {
        try
        {
            var result = await _tripService.ToggleTripStatusAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ToggleTripStatus endpoint for ID {TripId}", id);
            return StatusCode(500, ApiResponse<TripDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get trip statistics (Admin only)
    /// </summary>
    /// <returns>Trip statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TripDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<TripDto>>> GetTripStatistics()
    {
        try
        {
            var result = await _tripService.GetTripStatisticsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetTripStatistics endpoint");
            return StatusCode(500, ApiResponse<TripDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }
}
