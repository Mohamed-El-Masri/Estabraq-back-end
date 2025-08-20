using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstabraqTourismAPI.Services;
using EstabraqTourismAPI.DTOs.Auth;
using EstabraqTourismAPI.DTOs.User;
using EstabraqTourismAPI.DTOs.Common;
using EstabraqTourismAPI.Helpers;

namespace EstabraqTourismAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 400)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _authService.LoginAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Login endpoint");
            return StatusCode(500, ApiResponse<AuthResponseDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 400)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _authService.RegisterAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Register endpoint");
            return StatusCode(500, ApiResponse<AuthResponseDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 400)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<AuthResponseDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _authService.RefreshTokenAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshToken endpoint");
            return StatusCode(500, ApiResponse<AuthResponseDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="request">Current and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<string>>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<string>.FailureResult(
                    "Validation failed", errors));
            }

            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<string>.FailureResult("User not authenticated"));
            }

            var result = await _authService.ChangePasswordAsync(userId.Value, request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChangePassword endpoint");
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<UserDto>.FailureResult("User not authenticated"));
            }

            var result = await _authService.GetCurrentUserAsync(userId.Value);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCurrentUser endpoint");
            return StatusCode(500, ApiResponse<UserDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="request">Updated profile data</param>
    /// <returns>Updated user information</returns>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateProfileRequestDto request)
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

            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<UserDto>.FailureResult("User not authenticated"));
            }

            var result = await _authService.UpdateProfileAsync(userId.Value, request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateProfile endpoint");
            return StatusCode(500, ApiResponse<UserDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Logout user (client-side token invalidation)
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public Task<ActionResult<ApiResponse<string>>> Logout()
    {
        try
        {
            var userId = User.GetUserId();
            _logger.LogInformation("User {UserId} logged out", userId);
            
            return Task.FromResult<ActionResult<ApiResponse<string>>>(
                Ok(ApiResponse<string>.SuccessResult("", "Logged out successfully")));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Logout endpoint");
            return Task.FromResult<ActionResult<ApiResponse<string>>>(
                StatusCode(500, ApiResponse<string>.FailureResult(
                    "An error occurred while processing your request")));
        }
    }
}
