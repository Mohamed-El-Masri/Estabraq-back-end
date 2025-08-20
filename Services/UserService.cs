using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EstabraqTourismAPI.Data;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.User;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Services;

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(PaginationRequestDto parameters);
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequestDto request);
    Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserRequestDto request);
    Task<ApiResponse<string>> DeleteUserAsync(int id);
    Task<ApiResponse<string>> ToggleUserStatusAsync(int id);
    Task<ApiResponse<object>> GetUserStatisticsAsync();
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(PaginationRequestDto parameters)
    {
        try
        {
            var query = _context.Users.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(u => u.Name.Contains(parameters.Search) ||
                                        u.Email.Contains(parameters.Search));
            }

            // Sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "name":
                        query = parameters.SortDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name);
                        break;
                    case "email":
                        query = parameters.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                        break;
                    case "role":
                        query = parameters.SortDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role);
                        break;
                    case "createdat":
                        query = parameters.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt);
                        break;
                    default:
                        query = query.OrderByDescending(u => u.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(u => u.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);

            var result = new PagedResult<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            return ApiResponse<PagedResult<UserDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return ApiResponse<PagedResult<UserDto>>.FailureResult("An error occurred while getting users");
        }
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ApiResponse<UserDto>.FailureResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return ApiResponse<UserDto>.FailureResult("An error occurred while getting user");
        }
    }

    public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequestDto request)
    {
        try
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return ApiResponse<UserDto>.FailureResult("Email already exists");
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            _logger.LogInformation("User created with ID {UserId}", user.Id);
            return ApiResponse<UserDto>.SuccessResult(userDto, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return ApiResponse<UserDto>.FailureResult("An error occurred while creating user");
        }
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserRequestDto request)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ApiResponse<UserDto>.FailureResult("User not found");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != id);

            if (existingUser != null)
            {
                return ApiResponse<UserDto>.FailureResult("Email already exists");
            }

            _mapper.Map(request, user);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            _logger.LogInformation("User {UserId} updated successfully", id);
            return ApiResponse<UserDto>.SuccessResult(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return ApiResponse<UserDto>.FailureResult("An error occurred while updating user");
        }
    }

    public async Task<ApiResponse<string>> DeleteUserAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ApiResponse<string>.FailureResult("User not found");
            }

            // Check if user has bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.UserId == id);
            if (hasBookings)
            {
                return ApiResponse<string>.FailureResult("Cannot delete user with existing bookings");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} deleted successfully", id);
            return ApiResponse<string>.SuccessResult("", "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while deleting user");
        }
    }

    public async Task<ApiResponse<string>> ToggleUserStatusAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ApiResponse<string>.FailureResult("User not found");
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = user.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("User {UserId} {Status}", id, status);
            return ApiResponse<string>.SuccessResult("", $"User {status} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user status {UserId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while updating user status");
        }
    }

    public async Task<ApiResponse<object>> GetUserStatisticsAsync()
    {
        try
        {
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
            var recentUsers = await _context.Users.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));

            var stats = new
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = totalUsers - activeUsers,
                RecentUsers = recentUsers
            };

            return ApiResponse<object>.SuccessResult(stats, "User statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user statistics");
            return ApiResponse<object>.FailureResult("An error occurred while retrieving user statistics");
        }
    }
}
