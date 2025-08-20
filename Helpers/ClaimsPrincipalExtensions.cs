using System.Security.Claims;

namespace EstabraqTourismAPI.Helpers;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the claims
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>User ID or null if not found</returns>
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    /// <summary>
    /// Gets the user name from the claims
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>User name or null if not found</returns>
    public static string? GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Gets the user email from the claims
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>User email or null if not found</returns>
    public static string? GetUserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the user role from the claims
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>User role or null if not found</returns>
    public static string? GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value;
    }

    /// <summary>
    /// Checks if the user has a specific role
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role, false otherwise</returns>
    public static bool HasRole(this ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    /// <summary>
    /// Checks if the user is an admin
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if user is admin, false otherwise</returns>
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }

    /// <summary>
    /// Checks if the user is authenticated
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>True if user is authenticated, false otherwise</returns>
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Gets all claims for the user
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <returns>Dictionary of claims</returns>
    public static Dictionary<string, string> GetAllClaims(this ClaimsPrincipal user)
    {
        return user.Claims.ToDictionary(c => c.Type, c => c.Value);
    }

    /// <summary>
    /// Gets a specific claim value
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="claimType">Claim type to retrieve</param>
    /// <returns>Claim value or null if not found</returns>
    public static string? GetClaim(this ClaimsPrincipal user, string claimType)
    {
        return user.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Checks if the user owns the resource (same user ID)
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="resourceUserId">User ID of the resource owner</param>
    /// <returns>True if user owns the resource or is admin, false otherwise</returns>
    public static bool OwnsResource(this ClaimsPrincipal user, int resourceUserId)
    {
        var userId = user.GetUserId();
        return userId == resourceUserId || user.IsAdmin();
    }

    /// <summary>
    /// Checks if the user can access the resource (owns it or is admin)
    /// </summary>
    /// <param name="user">ClaimsPrincipal</param>
    /// <param name="resourceUserId">User ID of the resource owner</param>
    /// <returns>True if user can access the resource, false otherwise</returns>
    public static bool CanAccessResource(this ClaimsPrincipal user, int resourceUserId)
    {
        return user.OwnsResource(resourceUserId);
    }
}
