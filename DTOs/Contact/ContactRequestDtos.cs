using System.ComponentModel.DataAnnotations;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.DTOs.Contact;

// Request DTOs
public class ContactMessageRequestDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [Required]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;
}

public class GetContactMessagesRequestDto : PaginationRequestDto
{
    public bool? IsRead { get; set; }
    public bool? IsReplied { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public new string? SortBy { get; set; } = "createdAt"; // createdAt, name, subject
    public bool IsDescending { get; set; } = true;
}

public class ReplyToContactMessageRequestDto
{
    [Required]
    [StringLength(2000)]
    public string ReplyMessage { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string ReplySubject { get; set; } = string.Empty;
}

public class UpdateContactInfoRequestDto
{
    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress]
    public string? Phone2 { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email2 { get; set; }

    [Url]
    public string? Website { get; set; }

    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WhatsAppNumber { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public string? WorkingHours { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

// Response DTOs
public class ContactInfoDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Phone2 { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Email2 { get; set; }
    public string? Website { get; set; }
    public string? FacebookUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WhatsAppNumber { get; set; }
    public string? Description { get; set; }
    public string? WorkingHours { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ContactStatisticsDto
{
    public int TotalMessages { get; set; }
    public int UnreadMessages { get; set; }
    public int ReadMessages { get; set; }
    public int RepliedMessages { get; set; }
    public int UnrepliedMessages { get; set; }
    public int MessagesThisMonth { get; set; }
    public int MessagesLastMonth { get; set; }
    public double MessageGrowthRate { get; set; }
    public List<MonthlyMessageStatsDto> MonthlyStats { get; set; } = new();
    public DateTime LastMessageReceived { get; set; }
    public string? MostCommonSubject { get; set; }
}

public class MonthlyMessageStatsDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int MessageCount { get; set; }
    public int RepliedCount { get; set; }
    public string MonthName { get; set; } = string.Empty;
}
