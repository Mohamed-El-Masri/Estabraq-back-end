using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EstabraqTourismAPI.Data;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Category;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Services;

public interface ICategoryService
{
    Task<ApiResponse<PagedResult<CategoryDto>>> GetCategoriesAsync(PaginationRequestDto parameters);
    Task<ApiResponse<List<CategoryDto>>> GetActiveCategoriesAsync();
    Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<ApiResponse<CategoryDto>> GetCategoryBySlugAsync(string slug);
    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequestDto request);
    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request);
    Task<ApiResponse<string>> DeleteCategoryAsync(int id);
    Task<ApiResponse<string>> ToggleCategoryStatusAsync(int id);
    Task<ApiResponse<object>> GetCategoryStatisticsAsync();
}

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<CategoryService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<CategoryDto>>> GetCategoriesAsync(PaginationRequestDto parameters)
    {
        try
        {
            _logger.LogInformation("Starting GetCategoriesAsync with parameters: Page={Page}, PageSize={PageSize}, Search={Search}", 
                parameters.Page, parameters.PageSize, parameters.Search);

            // Test database connection first with detailed error info and timeout
            try
            {
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var canConnect = await _context.Database.CanConnectAsync(cancellationTokenSource.Token);
                _logger.LogInformation("Database connection test result: {CanConnect}", canConnect);
                
                if (!canConnect)
                {
                    _logger.LogError("Database connection failed - CanConnectAsync returned false");
                    return ApiResponse<PagedResult<CategoryDto>>.FailureResult("Database connection failed - Unable to connect");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Database connection test timed out after 30 seconds");
                return ApiResponse<PagedResult<CategoryDto>>.FailureResult("Database connection failed - Connection timeout");
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Database connection test threw exception: {ErrorType} - {Message}. Connection String: {ConnectionString}", 
                    dbEx.GetType().Name, dbEx.Message, 
                    _context.Database.GetConnectionString()?.Replace("Password=1HG?@t6kF2", "Password=***"));
                return ApiResponse<PagedResult<CategoryDto>>.FailureResult($"Database connection failed - {dbEx.Message}");
            }

            var query = _context.Categories
                .Include(c => c.Trips)
                .AsQueryable();

            _logger.LogInformation("Created initial query for categories");

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                _logger.LogInformation("Applying search filter: {Search}", parameters.Search);
                query = query.Where(c => c.Name.Contains(parameters.Search) ||
                                        c.NameAr.Contains(parameters.Search) ||
                                        c.Description!.Contains(parameters.Search) ||
                                        c.DescriptionAr!.Contains(parameters.Search));
            }

            // Sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "name":
                        query = parameters.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                        break;
                    case "namear":
                        query = parameters.SortDescending ? query.OrderByDescending(c => c.NameAr) : query.OrderBy(c => c.NameAr);
                        break;
                    case "createdat":
                        query = parameters.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt);
                        break;
                    default:
                        query = query.OrderByDescending(c => c.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            _logger.LogInformation("Executing count query for categories");
            var totalCount = await query.CountAsync();
            _logger.LogInformation("Total categories count: {TotalCount}", totalCount);

            _logger.LogInformation("Executing main query for categories with Skip={Skip}, Take={Take}", 
                (parameters.Page - 1) * parameters.PageSize, parameters.PageSize);
            
            var categories = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} categories from database", categories.Count);

            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            _logger.LogInformation("Mapped categories to DTOs successfully");

            var result = new PagedResult<CategoryDto>
            {
                Items = categoryDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            _logger.LogInformation("GetCategoriesAsync completed successfully");
            return ApiResponse<PagedResult<CategoryDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories. Exception details: {ExceptionType} - {Message}. StackTrace: {StackTrace}", 
                ex.GetType().Name, ex.Message, ex.StackTrace);
            return ApiResponse<PagedResult<CategoryDto>>.FailureResult("An error occurred while getting categories");
        }
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetActiveCategoriesAsync()
    {
        try
        {
            var categories = await _context.Categories
                .Include(c => c.Trips)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return ApiResponse<List<CategoryDto>>.SuccessResult(categoryDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active categories");
            return ApiResponse<List<CategoryDto>>.FailureResult("An error occurred while getting active categories");
        }
    }

    public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _context.Categories
                .Include(c => c.Trips)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return ApiResponse<CategoryDto>.FailureResult("Category not found");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.SuccessResult(categoryDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {CategoryId}", id);
            return ApiResponse<CategoryDto>.FailureResult("An error occurred while getting category");
        }
    }

    public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequestDto request)
    {
        try
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == request.Name.ToLower() ||
                                         c.NameAr == request.NameAr);

            if (existingCategory != null)
            {
                return ApiResponse<CategoryDto>.FailureResult("Category with this name already exists");
            }

            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = _mapper.Map<CategoryDto>(category);
            _logger.LogInformation("Category created with ID {CategoryId}", category.Id);
            return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Category created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return ApiResponse<CategoryDto>.FailureResult("An error occurred while creating category");
        }
    }

    public async Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryRequestDto request)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return ApiResponse<CategoryDto>.FailureResult("Category not found");
            }

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => (c.Name.ToLower() == request.Name.ToLower() ||
                                          c.NameAr == request.NameAr) && c.Id != id);

            if (existingCategory != null)
            {
                return ApiResponse<CategoryDto>.FailureResult("Category with this name already exists");
            }

            _mapper.Map(request, category);
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var categoryDto = _mapper.Map<CategoryDto>(category);
            _logger.LogInformation("Category {CategoryId} updated successfully", id);
            return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Category updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return ApiResponse<CategoryDto>.FailureResult("An error occurred while updating category");
        }
    }

    public async Task<ApiResponse<string>> DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _context.Categories
                .Include(c => c.Trips)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return ApiResponse<string>.FailureResult("Category not found");
            }

            if (category.Trips.Any())
            {
                return ApiResponse<string>.FailureResult("Cannot delete category with existing trips");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {CategoryId} deleted successfully", id);
            return ApiResponse<string>.SuccessResult("", "Category deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while deleting category");
        }
    }

    public async Task<ApiResponse<string>> ToggleCategoryStatusAsync(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return ApiResponse<string>.FailureResult("Category not found");
            }

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = category.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("Category {CategoryId} {Status}", id, status);
            return ApiResponse<string>.SuccessResult("", $"Category {status} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling category status {CategoryId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while updating category status");
        }
    }

    public async Task<ApiResponse<CategoryDto>> GetCategoryBySlugAsync(string slug)
    {
        try
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.Replace(" ", "-").ToLower() == slug.ToLower());

            if (category == null)
            {
                return ApiResponse<CategoryDto>.FailureResult("Category not found");
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Category retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by slug {Slug}", slug);
            return ApiResponse<CategoryDto>.FailureResult("An error occurred while retrieving category");
        }
    }

    public async Task<ApiResponse<object>> GetCategoryStatisticsAsync()
    {
        try
        {
            var totalCategories = await _context.Categories.CountAsync();
            var activeCategories = await _context.Categories.CountAsync(c => c.IsActive);

            var stats = new
            {
                TotalCategories = totalCategories,
                ActiveCategories = activeCategories,
                InactiveCategories = totalCategories - activeCategories
            };

            return ApiResponse<object>.SuccessResult(stats, "Category statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category statistics");
            return ApiResponse<object>.FailureResult("An error occurred while retrieving category statistics");
        }
    }
}
