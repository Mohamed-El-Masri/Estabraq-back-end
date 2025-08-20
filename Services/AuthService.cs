using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BCrypt.Net;
using EstabraqTourismAPI.Data;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Auth;
using EstabraqTourismAPI.DTOs.User;
using EstabraqTourismAPI.DTOs.Common;
using EstabraqTourismAPI.Configuration;

namespace EstabraqTourismAPI.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId);
    Task<ApiResponse<UserDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ApplicationDbContext context,
        IMapper mapper,
        JwtSettings jwtSettings,
        ILogger<AuthService> logger)
    {
        _context = context;
        _mapper = mapper;
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null || !user.IsActive)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Invalid email or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var userDto = _mapper.Map<UserDto>(user);
            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = userDto
            };

            _logger.LogInformation("User {Email} logged in successfully", request.Email);
            return ApiResponse<AuthResponseDto>.SuccessResult(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", request.Email);
            return ApiResponse<AuthResponseDto>.FailureResult("An error occurred during login");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return ApiResponse<AuthResponseDto>.FailureResult("Email already exists");
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var userDto = _mapper.Map<UserDto>(user);
            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                User = userDto
            };

            _logger.LogInformation("User {Email} registered successfully", request.Email);
            return ApiResponse<AuthResponseDto>.SuccessResult(response, "Registration successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", request.Email);
            return ApiResponse<AuthResponseDto>.FailureResult("An error occurred during registration");
        }
    }

    public Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        try
        {
            // In a real implementation, you would store refresh tokens in database
            // For now, we'll just validate the format and return a new token
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return Task.FromResult(ApiResponse<AuthResponseDto>.FailureResult("Invalid refresh token"));
            }

            // Here you would validate the refresh token against database
            // For this example, we'll assume it's valid and generate new tokens
            
            return Task.FromResult(ApiResponse<AuthResponseDto>.FailureResult("Refresh token functionality not implemented"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return Task.FromResult(ApiResponse<AuthResponseDto>.FailureResult("An error occurred during token refresh"));
        }
    }

    public async Task<ApiResponse<string>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<string>.FailureResult("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return ApiResponse<string>.FailureResult("Current password is incorrect");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Password changed for user {UserId}", userId);
            return ApiResponse<string>.SuccessResult("", "Password changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return ApiResponse<string>.FailureResult("An error occurred while changing password");
        }
    }

    public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserDto>.FailureResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponse<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", userId);
            return ApiResponse<UserDto>.FailureResult("An error occurred while getting user");
        }
    }

    public async Task<ApiResponse<UserDto>> UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserDto>.FailureResult("User not found");
            }

            _mapper.Map(request, user);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            _logger.LogInformation("Profile updated for user {UserId}", userId);
            return ApiResponse<UserDto>.SuccessResult(userDto, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            return ApiResponse<UserDto>.FailureResult("An error occurred while updating profile");
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}
