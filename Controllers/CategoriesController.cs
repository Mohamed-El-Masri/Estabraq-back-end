using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstabraqTourismAPI.Services;
using EstabraqTourismAPI.DTOs.Category;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Get all categories (Public)
    /// </summary>
    /// <param name="request">Pagination and search parameters</param>
    /// <returns>Paginated list of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetCategories([FromQuery] GetCategoriesRequestDto request)
    {
        try
        {
            _logger.LogInformation("GetCategories called with request: {@Request}", request);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState validation failed: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<PagedResult<CategoryDto>>.FailureResult(
                    "Validation failed", errors));
            }

            _logger.LogInformation("Calling category service to get categories");
            var result = await _categoryService.GetCategoriesAsync(request);
            
            if (!result.Success)
            {
                _logger.LogWarning("Category service returned failure: {Message}", result.Message);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully retrieved {Count} categories", result.Data?.Items?.Count ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in GetCategories: {ExceptionType} - {Message}", 
                ex.GetType().Name, ex.Message);
            return StatusCode(500, ApiResponse<PagedResult<CategoryDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get category by ID (Public)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 404)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(int id)
    {
        try
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCategory endpoint for ID {CategoryId}", id);
            return StatusCode(500, ApiResponse<CategoryDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get category by slug (Public)
    /// </summary>
    /// <param name="slug">Category slug</param>
    /// <returns>Category information</returns>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 404)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryBySlug(string slug)
    {
        try
        {
            var result = await _categoryService.GetCategoryBySlugAsync(slug);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCategoryBySlug endpoint for slug {CategorySlug}", slug);
            return StatusCode(500, ApiResponse<CategoryDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Create new category (Admin only)
    /// </summary>
    /// <param name="request">Category creation data</param>
    /// <returns>Created category information</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<CategoryDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _categoryService.CreateCategoryAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetCategory), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateCategory endpoint");
            return StatusCode(500, ApiResponse<CategoryDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Update category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Updated category data</param>
    /// <returns>Updated category information</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<CategoryDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _categoryService.UpdateCategoryAsync(id, request);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateCategory endpoint for ID {CategoryId}", id);
            return StatusCode(500, ApiResponse<CategoryDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Delete category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteCategory endpoint for ID {CategoryId}", id);
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Toggle category active status (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Updated category information</returns>
    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> ToggleCategoryStatus(int id)
    {
        try
        {
            var result = await _categoryService.ToggleCategoryStatusAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ToggleCategoryStatus endpoint for ID {CategoryId}", id);
            return StatusCode(500, ApiResponse<CategoryDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get category statistics (Admin only)
    /// </summary>
    /// <returns>Category statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryStatisticsDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<CategoryStatisticsDto>>> GetCategoryStatistics()
    {
        try
        {
            var result = await _categoryService.GetCategoryStatisticsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCategoryStatistics endpoint");
            return StatusCode(500, ApiResponse<CategoryStatisticsDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }
}
