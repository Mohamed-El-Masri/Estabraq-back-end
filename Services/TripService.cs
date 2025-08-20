using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EstabraqTourismAPI.Data;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Trip;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Services;

public interface ITripService
{
    Task<ApiResponse<PagedResult<TripSummaryDto>>> GetTripsAsync(PaginationRequestDto parameters);
    Task<ApiResponse<List<TripSummaryDto>>> GetFeaturedTripsAsync(int count = 6);
    Task<ApiResponse<List<TripSummaryDto>>> GetTripsByCategoryAsync(int categoryId, int count = 10);
    Task<ApiResponse<TripDto>> GetTripByIdAsync(int id);
    Task<ApiResponse<TripDto>> GetTripBySlugAsync(string slug);
    Task<ApiResponse<TripDto>> CreateTripAsync(CreateTripRequestDto request);
    Task<ApiResponse<TripDto>> UpdateTripAsync(int id, UpdateTripRequestDto request);
    Task<ApiResponse<string>> DeleteTripAsync(int id);
    Task<ApiResponse<string>> ToggleTripStatusAsync(int id);
    Task<ApiResponse<string>> ToggleFeaturedStatusAsync(int id);
    Task<ApiResponse<object>> GetTripStatisticsAsync();
    Task<ApiResponse<TripImageDto>> AddTripImageAsync(int tripId, CreateTripImageRequestDto request, string imageUrl);
    Task<ApiResponse<string>> DeleteTripImageAsync(int tripId, int imageId);
    Task<ApiResponse<TripScheduleDto>> AddTripScheduleAsync(int tripId, CreateTripScheduleRequestDto request);
    Task<ApiResponse<TripScheduleDto>> UpdateTripScheduleAsync(int tripId, int scheduleId, CreateTripScheduleRequestDto request);
    Task<ApiResponse<string>> DeleteTripScheduleAsync(int tripId, int scheduleId);
    Task<ApiResponse<TripIncludedDto>> AddTripIncludedAsync(int tripId, CreateTripIncludedRequestDto request);
    Task<ApiResponse<string>> DeleteTripIncludedAsync(int tripId, int includedId);
}

public class TripService : ITripService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TripService> _logger;

    public TripService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<TripService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<TripSummaryDto>>> GetTripsAsync(PaginationRequestDto parameters)
    {
        try
        {
            var query = _context.Trips
                .Include(t => t.Category)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(t => t.Title.Contains(parameters.Search) ||
                                        t.TitleAr.Contains(parameters.Search) ||
                                        t.Location.Contains(parameters.Search) ||
                                        t.LocationAr.Contains(parameters.Search) ||
                                        t.Description!.Contains(parameters.Search) ||
                                        t.DescriptionAr!.Contains(parameters.Search));
            }

            // Sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "title":
                        query = parameters.SortDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title);
                        break;
                    case "price":
                        query = parameters.SortDescending ? query.OrderByDescending(t => t.Price) : query.OrderBy(t => t.Price);
                        break;
                    case "duration":
                        query = parameters.SortDescending ? query.OrderByDescending(t => t.Duration) : query.OrderBy(t => t.Duration);
                        break;
                    case "location":
                        query = parameters.SortDescending ? query.OrderByDescending(t => t.Location) : query.OrderBy(t => t.Location);
                        break;
                    case "createdat":
                        query = parameters.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt);
                        break;
                    default:
                        query = query.OrderByDescending(t => t.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(t => t.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var trips = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var tripDtos = _mapper.Map<List<TripSummaryDto>>(trips);

            var result = new PagedResult<TripSummaryDto>
            {
                Items = tripDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            return ApiResponse<PagedResult<TripSummaryDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trips");
            return ApiResponse<PagedResult<TripSummaryDto>>.FailureResult("An error occurred while getting trips");
        }
    }

    public async Task<ApiResponse<List<TripSummaryDto>>> GetFeaturedTripsAsync(int count = 6)
    {
        try
        {
            var trips = await _context.Trips
                .Include(t => t.Category)
                .Where(t => t.IsActive && t.IsFeatured)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();

            var tripDtos = _mapper.Map<List<TripSummaryDto>>(trips);
            return ApiResponse<List<TripSummaryDto>>.SuccessResult(tripDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured trips");
            return ApiResponse<List<TripSummaryDto>>.FailureResult("An error occurred while getting featured trips");
        }
    }

    public async Task<ApiResponse<List<TripSummaryDto>>> GetTripsByCategoryAsync(int categoryId, int count = 10)
    {
        try
        {
            var trips = await _context.Trips
                .Include(t => t.Category)
                .Where(t => t.IsActive && t.CategoryId == categoryId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();

            var tripDtos = _mapper.Map<List<TripSummaryDto>>(trips);
            return ApiResponse<List<TripSummaryDto>>.SuccessResult(tripDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trips by category {CategoryId}", categoryId);
            return ApiResponse<List<TripSummaryDto>>.FailureResult("An error occurred while getting trips by category");
        }
    }

    public async Task<ApiResponse<TripDto>> GetTripByIdAsync(int id)
    {
        try
        {
            var trip = await _context.Trips
                .Include(t => t.Category)
                .Include(t => t.Images.OrderBy(i => i.SortOrder))
                .Include(t => t.Schedule.OrderBy(s => s.SortOrder))
                .Include(t => t.IncludedItems.OrderBy(i => i.SortOrder))
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
            {
                return ApiResponse<TripDto>.FailureResult("Trip not found");
            }

            var tripDto = _mapper.Map<TripDto>(trip);
            return ApiResponse<TripDto>.SuccessResult(tripDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trip {TripId}", id);
            return ApiResponse<TripDto>.FailureResult("An error occurred while getting trip");
        }
    }

    public async Task<ApiResponse<TripDto>> CreateTripAsync(CreateTripRequestDto request)
    {
        try
        {
            // Validate category exists
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<TripDto>.FailureResult("Category not found");
            }

            var trip = _mapper.Map<Trip>(request);
            trip.CreatedAt = DateTime.UtcNow;
            trip.UpdatedAt = DateTime.UtcNow;

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            // Load related data for response
            await _context.Entry(trip)
                .Reference(t => t.Category)
                .LoadAsync();

            var tripDto = _mapper.Map<TripDto>(trip);
            _logger.LogInformation("Trip created with ID {TripId}", trip.Id);
            return ApiResponse<TripDto>.SuccessResult(tripDto, "Trip created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating trip");
            return ApiResponse<TripDto>.FailureResult("An error occurred while creating trip");
        }
    }

    public async Task<ApiResponse<TripDto>> UpdateTripAsync(int id, UpdateTripRequestDto request)
    {
        try
        {
            var trip = await _context.Trips
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
            {
                return ApiResponse<TripDto>.FailureResult("Trip not found");
            }

            // Validate category exists
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                return ApiResponse<TripDto>.FailureResult("Category not found");
            }

            _mapper.Map(request, trip);
            trip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var tripDto = _mapper.Map<TripDto>(trip);
            _logger.LogInformation("Trip {TripId} updated successfully", id);
            return ApiResponse<TripDto>.SuccessResult(tripDto, "Trip updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating trip {TripId}", id);
            return ApiResponse<TripDto>.FailureResult("An error occurred while updating trip");
        }
    }

    public async Task<ApiResponse<string>> DeleteTripAsync(int id)
    {
        try
        {
            var trip = await _context.Trips
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
            {
                return ApiResponse<string>.FailureResult("Trip not found");
            }

            if (trip.Bookings.Any())
            {
                return ApiResponse<string>.FailureResult("Cannot delete trip with existing bookings");
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Trip {TripId} deleted successfully", id);
            return ApiResponse<string>.SuccessResult("", "Trip deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting trip {TripId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while deleting trip");
        }
    }

    public async Task<ApiResponse<string>> ToggleTripStatusAsync(int id)
    {
        try
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return ApiResponse<string>.FailureResult("Trip not found");
            }

            trip.IsActive = !trip.IsActive;
            trip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = trip.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("Trip {TripId} {Status}", id, status);
            return ApiResponse<string>.SuccessResult("", $"Trip {status} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling trip status {TripId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while updating trip status");
        }
    }

    public async Task<ApiResponse<string>> ToggleFeaturedStatusAsync(int id)
    {
        try
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return ApiResponse<string>.FailureResult("Trip not found");
            }

            trip.IsFeatured = !trip.IsFeatured;
            trip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = trip.IsFeatured ? "featured" : "unfeatured";
            _logger.LogInformation("Trip {TripId} {Status}", id, status);
            return ApiResponse<string>.SuccessResult("", $"Trip {status} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling trip featured status {TripId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while updating trip featured status");
        }
    }

    public async Task<ApiResponse<TripImageDto>> AddTripImageAsync(int tripId, CreateTripImageRequestDto request, string imageUrl)
    {
        try
        {
            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null)
            {
                return ApiResponse<TripImageDto>.FailureResult("Trip not found");
            }

            var tripImage = _mapper.Map<TripImage>(request);
            tripImage.TripId = tripId;
            tripImage.ImageUrl = imageUrl;
            tripImage.CreatedAt = DateTime.UtcNow;

            _context.TripImages.Add(tripImage);
            await _context.SaveChangesAsync();

            var tripImageDto = _mapper.Map<TripImageDto>(tripImage);
            _logger.LogInformation("Image added to trip {TripId}", tripId);
            return ApiResponse<TripImageDto>.SuccessResult(tripImageDto, "Image added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding image to trip {TripId}", tripId);
            return ApiResponse<TripImageDto>.FailureResult("An error occurred while adding image");
        }
    }

    public async Task<ApiResponse<string>> DeleteTripImageAsync(int tripId, int imageId)
    {
        try
        {
            var tripImage = await _context.TripImages
                .FirstOrDefaultAsync(i => i.Id == imageId && i.TripId == tripId);

            if (tripImage == null)
            {
                return ApiResponse<string>.FailureResult("Image not found");
            }

            _context.TripImages.Remove(tripImage);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Image {ImageId} deleted from trip {TripId}", imageId, tripId);
            return ApiResponse<string>.SuccessResult("", "Image deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ImageId} from trip {TripId}", imageId, tripId);
            return ApiResponse<string>.FailureResult("An error occurred while deleting image");
        }
    }

    public async Task<ApiResponse<TripScheduleDto>> AddTripScheduleAsync(int tripId, CreateTripScheduleRequestDto request)
    {
        try
        {
            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null)
            {
                return ApiResponse<TripScheduleDto>.FailureResult("Trip not found");
            }

            var tripSchedule = _mapper.Map<TripSchedule>(request);
            tripSchedule.TripId = tripId;
            tripSchedule.CreatedAt = DateTime.UtcNow;

            _context.TripSchedules.Add(tripSchedule);
            await _context.SaveChangesAsync();

            var tripScheduleDto = _mapper.Map<TripScheduleDto>(tripSchedule);
            _logger.LogInformation("Schedule added to trip {TripId}", tripId);
            return ApiResponse<TripScheduleDto>.SuccessResult(tripScheduleDto, "Schedule added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding schedule to trip {TripId}", tripId);
            return ApiResponse<TripScheduleDto>.FailureResult("An error occurred while adding schedule");
        }
    }

    public async Task<ApiResponse<TripScheduleDto>> UpdateTripScheduleAsync(int tripId, int scheduleId, CreateTripScheduleRequestDto request)
    {
        try
        {
            var tripSchedule = await _context.TripSchedules
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.TripId == tripId);

            if (tripSchedule == null)
            {
                return ApiResponse<TripScheduleDto>.FailureResult("Schedule not found");
            }

            _mapper.Map(request, tripSchedule);

            await _context.SaveChangesAsync();

            var tripScheduleDto = _mapper.Map<TripScheduleDto>(tripSchedule);
            _logger.LogInformation("Schedule {ScheduleId} updated for trip {TripId}", scheduleId, tripId);
            return ApiResponse<TripScheduleDto>.SuccessResult(tripScheduleDto, "Schedule updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating schedule {ScheduleId} for trip {TripId}", scheduleId, tripId);
            return ApiResponse<TripScheduleDto>.FailureResult("An error occurred while updating schedule");
        }
    }

    public async Task<ApiResponse<string>> DeleteTripScheduleAsync(int tripId, int scheduleId)
    {
        try
        {
            var tripSchedule = await _context.TripSchedules
                .FirstOrDefaultAsync(s => s.Id == scheduleId && s.TripId == tripId);

            if (tripSchedule == null)
            {
                return ApiResponse<string>.FailureResult("Schedule not found");
            }

            _context.TripSchedules.Remove(tripSchedule);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Schedule {ScheduleId} deleted from trip {TripId}", scheduleId, tripId);
            return ApiResponse<string>.SuccessResult("", "Schedule deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting schedule {ScheduleId} from trip {TripId}", scheduleId, tripId);
            return ApiResponse<string>.FailureResult("An error occurred while deleting schedule");
        }
    }

    public async Task<ApiResponse<TripIncludedDto>> AddTripIncludedAsync(int tripId, CreateTripIncludedRequestDto request)
    {
        try
        {
            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null)
            {
                return ApiResponse<TripIncludedDto>.FailureResult("Trip not found");
            }

            var tripIncluded = _mapper.Map<TripIncluded>(request);
            tripIncluded.TripId = tripId;
            tripIncluded.CreatedAt = DateTime.UtcNow;

            _context.TripIncluded.Add(tripIncluded);
            await _context.SaveChangesAsync();

            var tripIncludedDto = _mapper.Map<TripIncludedDto>(tripIncluded);
            _logger.LogInformation("Included item added to trip {TripId}", tripId);
            return ApiResponse<TripIncludedDto>.SuccessResult(tripIncludedDto, "Included item added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding included item to trip {TripId}", tripId);
            return ApiResponse<TripIncludedDto>.FailureResult("An error occurred while adding included item");
        }
    }

    public async Task<ApiResponse<string>> DeleteTripIncludedAsync(int tripId, int includedId)
    {
        try
        {
            var tripIncluded = await _context.TripIncluded
                .FirstOrDefaultAsync(i => i.Id == includedId && i.TripId == tripId);

            if (tripIncluded == null)
            {
                return ApiResponse<string>.FailureResult("Included item not found");
            }

            _context.TripIncluded.Remove(tripIncluded);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Included item {IncludedId} deleted from trip {TripId}", includedId, tripId);
            return ApiResponse<string>.SuccessResult("", "Included item deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting included item {IncludedId} from trip {TripId}", includedId, tripId);
            return ApiResponse<string>.FailureResult("An error occurred while deleting included item");
        }
    }

    public async Task<ApiResponse<TripDto>> GetTripBySlugAsync(string slug)
    {
        try
        {
            var trip = await _context.Trips
                .Include(t => t.Category)
                .Include(t => t.Images)
                .Include(t => t.Schedule)
                .Include(t => t.IncludedItems)
                .FirstOrDefaultAsync(t => t.Title.Replace(" ", "-").ToLower() == slug.ToLower());

            if (trip == null)
            {
                return ApiResponse<TripDto>.FailureResult("Trip not found");
            }

            var tripDto = _mapper.Map<TripDto>(trip);
            return ApiResponse<TripDto>.SuccessResult(tripDto, "Trip retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trip by slug {Slug}", slug);
            return ApiResponse<TripDto>.FailureResult("An error occurred while retrieving trip");
        }
    }

    public async Task<ApiResponse<object>> GetTripStatisticsAsync()
    {
        try
        {
            var totalTrips = await _context.Trips.CountAsync();
            var activeTrips = await _context.Trips.CountAsync(t => t.IsActive);
            var featuredTrips = await _context.Trips.CountAsync(t => t.IsFeatured);
            var avgPrice = await _context.Trips.AverageAsync(t => t.Price);

            var stats = new
            {
                TotalTrips = totalTrips,
                ActiveTrips = activeTrips,
                InactiveTrips = totalTrips - activeTrips,
                FeaturedTrips = featuredTrips,
                AveragePrice = avgPrice
            };

            return ApiResponse<object>.SuccessResult(stats, "Trip statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trip statistics");
            return ApiResponse<object>.FailureResult("An error occurred while retrieving trip statistics");
        }
    }
}
