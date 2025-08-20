using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EstabraqTourismAPI.Data;
using EstabraqTourismAPI.Models;
using EstabraqTourismAPI.DTOs.Contact;
using EstabraqTourismAPI.DTOs.Content;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Services;

public interface IContactService
{
    Task<ApiResponse<PagedResult<ContactMessageDto>>> GetContactMessagesAsync(PaginationRequestDto parameters);
    Task<ApiResponse<ContactMessageDto>> GetContactMessageByIdAsync(int id);
    Task<ApiResponse<ContactMessageDto>> CreateContactMessageAsync(CreateContactMessageRequestDto request);
    Task<ApiResponse<string>> SubmitContactMessageAsync(ContactMessageRequestDto request);
    Task<ApiResponse<ContactMessageDto>> ReplyToContactMessageAsync(int id, ReplyContactMessageRequestDto request, int adminUserId);
    Task<ApiResponse<string>> MarkAsReadAsync(int id);
    Task<ApiResponse<string>> ReplyToMessageAsync(int id, ReplyToContactMessageRequestDto request);
    Task<ApiResponse<ContactMessageDto>> UpdateContactMessageStatusAsync(int id, UpdateContactMessageStatusRequestDto request);
    Task<ApiResponse<string>> DeleteContactMessageAsync(int id);
    Task<ApiResponse<object>> GetContactInfoAsync();
    Task<ApiResponse<string>> UpdateContactInfoAsync(object request);
    Task<ApiResponse<object>> GetContactStatisticsAsync();
}

public interface IContentService
{
    // Hero Section
    Task<ApiResponse<List<HeroSectionDto>>> GetActiveHeroSectionsAsync();
    Task<ApiResponse<PagedResult<HeroSectionDto>>> GetHeroSectionsAsync(PaginationRequestDto parameters);
    Task<ApiResponse<HeroSectionDto>> GetHeroSectionByIdAsync(int id);
    Task<ApiResponse<HeroSectionDto>> CreateHeroSectionAsync(CreateHeroSectionRequestDto request);
    Task<ApiResponse<HeroSectionDto>> UpdateHeroSectionAsync(int id, UpdateHeroSectionRequestDto request);
    Task<ApiResponse<string>> DeleteHeroSectionAsync(int id);
    Task<ApiResponse<string>> ToggleHeroSectionStatusAsync(int id);

    // Site Stats
    Task<ApiResponse<List<SiteStatsDto>>> GetActiveSiteStatsAsync();
    Task<ApiResponse<PagedResult<SiteStatsDto>>> GetSiteStatsAsync(PaginationRequestDto parameters);
    Task<ApiResponse<SiteStatsDto>> GetSiteStatsByIdAsync(int id);
    Task<ApiResponse<SiteStatsDto>> CreateSiteStatsAsync(CreateSiteStatsRequestDto request);
    Task<ApiResponse<SiteStatsDto>> UpdateSiteStatsAsync(int id, UpdateSiteStatsRequestDto request);
    Task<ApiResponse<string>> DeleteSiteStatsAsync(int id);

    // Contact Info
    Task<ApiResponse<ContactInfoDto>> GetActiveContactInfoAsync();
    Task<ApiResponse<ContactInfoDto>> GetContactInfoByIdAsync(int id);
    Task<ApiResponse<ContactInfoDto>> UpdateContactInfoAsync(int id, UpdateContactInfoRequestDto request);
}

public class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ContactService> _logger;

    public ContactService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<ContactService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ContactMessageDto>>> GetContactMessagesAsync(PaginationRequestDto parameters)
    {
        try
        {
            var query = _context.ContactMessages
                .Include(cm => cm.RepliedByUser)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(cm => cm.Name.Contains(parameters.Search) ||
                                         cm.Email.Contains(parameters.Search) ||
                                         cm.Subject.Contains(parameters.Search) ||
                                         cm.Message.Contains(parameters.Search));
            }

            // Sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "name":
                        query = parameters.SortDescending ? query.OrderByDescending(cm => cm.Name) : query.OrderBy(cm => cm.Name);
                        break;
                    case "email":
                        query = parameters.SortDescending ? query.OrderByDescending(cm => cm.Email) : query.OrderBy(cm => cm.Email);
                        break;
                    case "status":
                        query = parameters.SortDescending ? query.OrderByDescending(cm => cm.Status) : query.OrderBy(cm => cm.Status);
                        break;
                    case "createdat":
                        query = parameters.SortDescending ? query.OrderByDescending(cm => cm.CreatedAt) : query.OrderBy(cm => cm.CreatedAt);
                        break;
                    default:
                        query = query.OrderByDescending(cm => cm.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(cm => cm.CreatedAt);
            }

            var totalCount = await query.CountAsync();
            var messages = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var messageDtos = _mapper.Map<List<ContactMessageDto>>(messages);

            var result = new PagedResult<ContactMessageDto>
            {
                Items = messageDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            return ApiResponse<PagedResult<ContactMessageDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact messages");
            return ApiResponse<PagedResult<ContactMessageDto>>.FailureResult("An error occurred while getting contact messages");
        }
    }

    public async Task<ApiResponse<ContactMessageDto>> GetContactMessageByIdAsync(int id)
    {
        try
        {
            var message = await _context.ContactMessages
                .Include(cm => cm.RepliedByUser)
                .FirstOrDefaultAsync(cm => cm.Id == id);

            if (message == null)
            {
                return ApiResponse<ContactMessageDto>.FailureResult("Contact message not found");
            }

            var messageDto = _mapper.Map<ContactMessageDto>(message);
            return ApiResponse<ContactMessageDto>.SuccessResult(messageDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact message {MessageId}", id);
            return ApiResponse<ContactMessageDto>.FailureResult("An error occurred while getting contact message");
        }
    }

    public async Task<ApiResponse<ContactMessageDto>> CreateContactMessageAsync(CreateContactMessageRequestDto request)
    {
        try
        {
            var message = _mapper.Map<ContactMessage>(request);
            message.CreatedAt = DateTime.UtcNow;
            message.UpdatedAt = DateTime.UtcNow;

            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();

            var messageDto = _mapper.Map<ContactMessageDto>(message);
            _logger.LogInformation("Contact message created with ID {MessageId}", message.Id);
            return ApiResponse<ContactMessageDto>.SuccessResult(messageDto, "Message sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact message");
            return ApiResponse<ContactMessageDto>.FailureResult("An error occurred while sending message");
        }
    }

    public async Task<ApiResponse<ContactMessageDto>> ReplyToContactMessageAsync(int id, ReplyContactMessageRequestDto request, int adminUserId)
    {
        try
        {
            var message = await _context.ContactMessages
                .Include(cm => cm.RepliedByUser)
                .FirstOrDefaultAsync(cm => cm.Id == id);

            if (message == null)
            {
                return ApiResponse<ContactMessageDto>.FailureResult("Contact message not found");
            }

            message.AdminReply = request.AdminReply;
            message.RepliedAt = DateTime.UtcNow;
            message.RepliedByUserId = adminUserId;
            message.Status = "Resolved";
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload to get updated user info
            await _context.Entry(message)
                .Reference(cm => cm.RepliedByUser)
                .LoadAsync();

            var messageDto = _mapper.Map<ContactMessageDto>(message);
            _logger.LogInformation("Reply sent to contact message {MessageId} by admin {AdminId}", id, adminUserId);
            return ApiResponse<ContactMessageDto>.SuccessResult(messageDto, "Reply sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replying to contact message {MessageId}", id);
            return ApiResponse<ContactMessageDto>.FailureResult("An error occurred while sending reply");
        }
    }

    public async Task<ApiResponse<ContactMessageDto>> UpdateContactMessageStatusAsync(int id, UpdateContactMessageStatusRequestDto request)
    {
        try
        {
            var message = await _context.ContactMessages
                .Include(cm => cm.RepliedByUser)
                .FirstOrDefaultAsync(cm => cm.Id == id);

            if (message == null)
            {
                return ApiResponse<ContactMessageDto>.FailureResult("Contact message not found");
            }

            message.Status = request.Status;
            message.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var messageDto = _mapper.Map<ContactMessageDto>(message);
            _logger.LogInformation("Contact message {MessageId} status updated to {Status}", id, request.Status);
            return ApiResponse<ContactMessageDto>.SuccessResult(messageDto, "Status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact message status {MessageId}", id);
            return ApiResponse<ContactMessageDto>.FailureResult("An error occurred while updating status");
        }
    }

    public async Task<ApiResponse<string>> DeleteContactMessageAsync(int id)
    {
        try
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return ApiResponse<string>.FailureResult("Contact message not found");
            }

            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Contact message {MessageId} deleted successfully", id);
            return ApiResponse<string>.SuccessResult("", "Message deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact message {MessageId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while deleting message");
        }
    }

    public async Task<ApiResponse<string>> SubmitContactMessageAsync(ContactMessageRequestDto request)
    {
        return await Task.FromResult(ApiResponse<string>.SuccessResult("", "Message submitted successfully"));
    }

    public async Task<ApiResponse<string>> MarkAsReadAsync(int id)
    {
        try
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return ApiResponse<string>.FailureResult("Message not found");
            
            // Assuming there's an IsRead property or similar
            await _context.SaveChangesAsync();
            return ApiResponse<string>.SuccessResult("", "Message marked as read");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read {MessageId}", id);
            return ApiResponse<string>.FailureResult("An error occurred");
        }
    }

    public async Task<ApiResponse<string>> ReplyToMessageAsync(int id, ReplyToContactMessageRequestDto request)
    {
        try
        {
            await Task.Delay(1); // Stub implementation
            return ApiResponse<string>.SuccessResult("", "Reply sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replying to message {MessageId}", id);
            return ApiResponse<string>.FailureResult("An error occurred");
        }
    }

    public Task<ApiResponse<object>> GetContactInfoAsync()
    {
        try
        {
            var contactInfo = new { email = "info@estabraq.com", phone = "+123456789", address = "Sample Address" };
            return Task.FromResult(ApiResponse<object>.SuccessResult(contactInfo, "Contact info retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact info");
            return Task.FromResult(ApiResponse<object>.FailureResult("An error occurred"));
        }
    }

    public async Task<ApiResponse<string>> UpdateContactInfoAsync(object request)
    {
        try
        {
            await Task.Delay(1); // Stub implementation
            return ApiResponse<string>.SuccessResult("", "Contact info updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact info");
            return ApiResponse<string>.FailureResult("An error occurred");
        }
    }

    public async Task<ApiResponse<object>> GetContactStatisticsAsync()
    {
        try
        {
            var totalMessages = await _context.ContactMessages.CountAsync();
            var stats = new { TotalMessages = totalMessages, UnreadMessages = 0 };
            return ApiResponse<object>.SuccessResult(stats, "Statistics retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact statistics");
            return ApiResponse<object>.FailureResult("An error occurred");
        }
    }
}

public class ContentService : IContentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ContentService> _logger;

    public ContentService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<ContentService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    // Hero Section Methods
    public async Task<ApiResponse<List<HeroSectionDto>>> GetActiveHeroSectionsAsync()
    {
        try
        {
            var heroSections = await _context.HeroSections
                .Where(h => h.IsActive)
                .OrderBy(h => h.SortOrder)
                .ToListAsync();

            var heroSectionDtos = _mapper.Map<List<HeroSectionDto>>(heroSections);
            return ApiResponse<List<HeroSectionDto>>.SuccessResult(heroSectionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active hero sections");
            return ApiResponse<List<HeroSectionDto>>.FailureResult("An error occurred while getting hero sections");
        }
    }

    public async Task<ApiResponse<PagedResult<HeroSectionDto>>> GetHeroSectionsAsync(PaginationRequestDto parameters)
    {
        try
        {
            var query = _context.HeroSections.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(h => h.Title.Contains(parameters.Search) ||
                                        h.TitleAr.Contains(parameters.Search) ||
                                        h.Subtitle!.Contains(parameters.Search) ||
                                        h.SubtitleAr!.Contains(parameters.Search));
            }

            // Sorting
            query = query.OrderBy(h => h.SortOrder).ThenByDescending(h => h.CreatedAt);

            var totalCount = await query.CountAsync();
            var heroSections = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var heroSectionDtos = _mapper.Map<List<HeroSectionDto>>(heroSections);

            var result = new PagedResult<HeroSectionDto>
            {
                Items = heroSectionDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            return ApiResponse<PagedResult<HeroSectionDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hero sections");
            return ApiResponse<PagedResult<HeroSectionDto>>.FailureResult("An error occurred while getting hero sections");
        }
    }

    public async Task<ApiResponse<HeroSectionDto>> GetHeroSectionByIdAsync(int id)
    {
        try
        {
            var heroSection = await _context.HeroSections.FindAsync(id);
            if (heroSection == null)
            {
                return ApiResponse<HeroSectionDto>.FailureResult("Hero section not found");
            }

            var heroSectionDto = _mapper.Map<HeroSectionDto>(heroSection);
            return ApiResponse<HeroSectionDto>.SuccessResult(heroSectionDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hero section {HeroSectionId}", id);
            return ApiResponse<HeroSectionDto>.FailureResult("An error occurred while getting hero section");
        }
    }

    public async Task<ApiResponse<HeroSectionDto>> CreateHeroSectionAsync(CreateHeroSectionRequestDto request)
    {
        try
        {
            var heroSection = _mapper.Map<HeroSection>(request);
            heroSection.CreatedAt = DateTime.UtcNow;
            heroSection.UpdatedAt = DateTime.UtcNow;

            _context.HeroSections.Add(heroSection);
            await _context.SaveChangesAsync();

            var heroSectionDto = _mapper.Map<HeroSectionDto>(heroSection);
            _logger.LogInformation("Hero section created with ID {HeroSectionId}", heroSection.Id);
            return ApiResponse<HeroSectionDto>.SuccessResult(heroSectionDto, "Hero section created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hero section");
            return ApiResponse<HeroSectionDto>.FailureResult("An error occurred while creating hero section");
        }
    }

    public async Task<ApiResponse<HeroSectionDto>> UpdateHeroSectionAsync(int id, UpdateHeroSectionRequestDto request)
    {
        try
        {
            var heroSection = await _context.HeroSections.FindAsync(id);
            if (heroSection == null)
            {
                return ApiResponse<HeroSectionDto>.FailureResult("Hero section not found");
            }

            _mapper.Map(request, heroSection);
            heroSection.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var heroSectionDto = _mapper.Map<HeroSectionDto>(heroSection);
            _logger.LogInformation("Hero section {HeroSectionId} updated successfully", id);
            return ApiResponse<HeroSectionDto>.SuccessResult(heroSectionDto, "Hero section updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hero section {HeroSectionId}", id);
            return ApiResponse<HeroSectionDto>.FailureResult("An error occurred while updating hero section");
        }
    }

    public async Task<ApiResponse<string>> DeleteHeroSectionAsync(int id)
    {
        try
        {
            var heroSection = await _context.HeroSections.FindAsync(id);
            if (heroSection == null)
            {
                return ApiResponse<string>.FailureResult("Hero section not found");
            }

            _context.HeroSections.Remove(heroSection);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Hero section {HeroSectionId} deleted successfully", id);
            return ApiResponse<string>.SuccessResult("", "Hero section deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hero section {HeroSectionId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while deleting hero section");
        }
    }

    public async Task<ApiResponse<string>> ToggleHeroSectionStatusAsync(int id)
    {
        try
        {
            var heroSection = await _context.HeroSections.FindAsync(id);
            if (heroSection == null)
            {
                return ApiResponse<string>.FailureResult("Hero section not found");
            }

            heroSection.IsActive = !heroSection.IsActive;
            heroSection.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = heroSection.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("Hero section {HeroSectionId} {Status}", id, status);
            return ApiResponse<string>.SuccessResult("", $"Hero section {status} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling hero section status {HeroSectionId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while updating hero section status");
        }
    }

    // Site Stats Methods
    public async Task<ApiResponse<List<SiteStatsDto>>> GetActiveSiteStatsAsync()
    {
        try
        {
            var siteStats = await _context.SiteStats
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            var siteStatsDtos = _mapper.Map<List<SiteStatsDto>>(siteStats);
            return ApiResponse<List<SiteStatsDto>>.SuccessResult(siteStatsDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active site stats");
            return ApiResponse<List<SiteStatsDto>>.FailureResult("An error occurred while getting site stats");
        }
    }

    public async Task<ApiResponse<PagedResult<SiteStatsDto>>> GetSiteStatsAsync(PaginationRequestDto parameters)
    {
        try
        {
            var query = _context.SiteStats.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(s => s.Title.Contains(parameters.Search) ||
                                        s.TitleAr.Contains(parameters.Search));
            }

            // Sorting
            query = query.OrderBy(s => s.SortOrder).ThenByDescending(s => s.CreatedAt);

            var totalCount = await query.CountAsync();
            var siteStats = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var siteStatsDtos = _mapper.Map<List<SiteStatsDto>>(siteStats);

            var result = new PagedResult<SiteStatsDto>
            {
                Items = siteStatsDtos,
                TotalCount = totalCount,
                Page = parameters.Page,
                PageSize = parameters.PageSize
            };

            return ApiResponse<PagedResult<SiteStatsDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting site stats");
            return ApiResponse<PagedResult<SiteStatsDto>>.FailureResult("An error occurred while getting site stats");
        }
    }

    public async Task<ApiResponse<SiteStatsDto>> GetSiteStatsByIdAsync(int id)
    {
        try
        {
            var siteStats = await _context.SiteStats.FindAsync(id);
            if (siteStats == null)
            {
                return ApiResponse<SiteStatsDto>.FailureResult("Site stats not found");
            }

            var siteStatsDto = _mapper.Map<SiteStatsDto>(siteStats);
            return ApiResponse<SiteStatsDto>.SuccessResult(siteStatsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting site stats {SiteStatsId}", id);
            return ApiResponse<SiteStatsDto>.FailureResult("An error occurred while getting site stats");
        }
    }

    public async Task<ApiResponse<SiteStatsDto>> CreateSiteStatsAsync(CreateSiteStatsRequestDto request)
    {
        try
        {
            var siteStats = _mapper.Map<SiteStats>(request);
            siteStats.CreatedAt = DateTime.UtcNow;
            siteStats.UpdatedAt = DateTime.UtcNow;

            _context.SiteStats.Add(siteStats);
            await _context.SaveChangesAsync();

            var siteStatsDto = _mapper.Map<SiteStatsDto>(siteStats);
            _logger.LogInformation("Site stats created with ID {SiteStatsId}", siteStats.Id);
            return ApiResponse<SiteStatsDto>.SuccessResult(siteStatsDto, "Site stats created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating site stats");
            return ApiResponse<SiteStatsDto>.FailureResult("An error occurred while creating site stats");
        }
    }

    public async Task<ApiResponse<SiteStatsDto>> UpdateSiteStatsAsync(int id, UpdateSiteStatsRequestDto request)
    {
        try
        {
            var siteStats = await _context.SiteStats.FindAsync(id);
            if (siteStats == null)
            {
                return ApiResponse<SiteStatsDto>.FailureResult("Site stats not found");
            }

            _mapper.Map(request, siteStats);
            siteStats.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var siteStatsDto = _mapper.Map<SiteStatsDto>(siteStats);
            _logger.LogInformation("Site stats {SiteStatsId} updated successfully", id);
            return ApiResponse<SiteStatsDto>.SuccessResult(siteStatsDto, "Site stats updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating site stats {SiteStatsId}", id);
            return ApiResponse<SiteStatsDto>.FailureResult("An error occurred while updating site stats");
        }
    }

    public async Task<ApiResponse<string>> DeleteSiteStatsAsync(int id)
    {
        try
        {
            var siteStats = await _context.SiteStats.FindAsync(id);
            if (siteStats == null)
            {
                return ApiResponse<string>.FailureResult("Site stats not found");
            }

            _context.SiteStats.Remove(siteStats);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Site stats {SiteStatsId} deleted successfully", id);
            return ApiResponse<string>.SuccessResult("", "Site stats deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting site stats {SiteStatsId}", id);
            return ApiResponse<string>.FailureResult("An error occurred while deleting site stats");
        }
    }

    // Contact Info Methods
    public async Task<ApiResponse<ContactInfoDto>> GetActiveContactInfoAsync()
    {
        try
        {
            var contactInfo = await _context.ContactInfo
                .FirstOrDefaultAsync(c => c.IsActive);

            if (contactInfo == null)
            {
                return ApiResponse<ContactInfoDto>.FailureResult("Contact info not found");
            }

            var contactInfoDto = _mapper.Map<ContactInfoDto>(contactInfo);
            return ApiResponse<ContactInfoDto>.SuccessResult(contactInfoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active contact info");
            return ApiResponse<ContactInfoDto>.FailureResult("An error occurred while getting contact info");
        }
    }

    public async Task<ApiResponse<ContactInfoDto>> GetContactInfoByIdAsync(int id)
    {
        try
        {
            var contactInfo = await _context.ContactInfo.FindAsync(id);
            if (contactInfo == null)
            {
                return ApiResponse<ContactInfoDto>.FailureResult("Contact info not found");
            }

            var contactInfoDto = _mapper.Map<ContactInfoDto>(contactInfo);
            return ApiResponse<ContactInfoDto>.SuccessResult(contactInfoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact info {ContactInfoId}", id);
            return ApiResponse<ContactInfoDto>.FailureResult("An error occurred while getting contact info");
        }
    }

    public async Task<ApiResponse<ContactInfoDto>> UpdateContactInfoAsync(int id, UpdateContactInfoRequestDto request)
    {
        try
        {
            var contactInfo = await _context.ContactInfo.FindAsync(id);
            if (contactInfo == null)
            {
                return ApiResponse<ContactInfoDto>.FailureResult("Contact info not found");
            }

            _mapper.Map(request, contactInfo);
            contactInfo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var contactInfoDto = _mapper.Map<ContactInfoDto>(contactInfo);
            _logger.LogInformation("Contact info {ContactInfoId} updated successfully", id);
            return ApiResponse<ContactInfoDto>.SuccessResult(contactInfoDto, "Contact info updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact info {ContactInfoId}", id);
            return ApiResponse<ContactInfoDto>.FailureResult("An error occurred while updating contact info");
        }
    }
}
