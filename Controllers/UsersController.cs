using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstabraqTourismAPI.Services;
using EstabraqTourismAPI.DTOs.User;
using EstabraqTourismAPI.DTOs.Common;
using EstabraqTourismAPI.Helpers;

namespace EstabraqTourismAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    /// <param name="request">Pagination and search parameters</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers([FromQuery] GetUsersRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<PagedResult<UserDto>>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _userService.GetUsersAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUsers endpoint");
            return StatusCode(500, ApiResponse<PagedResult<UserDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        try
        {
            var result = await _userService.GetUserByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUser endpoint for ID {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Create new user (Admin only)
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user information</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<UserDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _userService.CreateUserAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateUser endpoint");
            return StatusCode(500, ApiResponse<UserDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Update user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Updated user data</param>
    /// <returns>Updated user information</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<UserDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _userService.UpdateUserAsync(id, request);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateUser endpoint for ID {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<string>>> DeleteUser(int id)
    {
        try
        {
            var currentUserId = User.GetUserId();
            if (currentUserId == id)
            {
                return BadRequest(ApiResponse<string>.FailureResult(
                    "Cannot delete your own account"));
            }

            var result = await _userService.DeleteUserAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteUser endpoint for ID {UserId}", id);
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Toggle user active status (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Updated user information</returns>
    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UserDto>>> ToggleUserStatus(int id)
    {
        try
        {
            var currentUserId = User.GetUserId();
            if (currentUserId == id)
            {
                return BadRequest(ApiResponse<UserDto>.FailureResult(
                    "Cannot change your own status"));
            }

            var result = await _userService.ToggleUserStatusAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ToggleUserStatus endpoint for ID {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get user statistics (Admin only)
    /// </summary>
    /// <returns>User statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserStatisticsDto>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<UserStatisticsDto>>> GetUserStatistics()
    {
        try
        {
            var result = await _userService.GetUserStatisticsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserStatistics endpoint");
            return StatusCode(500, ApiResponse<UserStatisticsDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }
}
