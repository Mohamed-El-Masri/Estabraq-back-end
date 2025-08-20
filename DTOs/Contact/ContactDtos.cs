using System.ComponentModel.DataAnnotations;

namespace EstabraqTourismAPI.DTOs.Contact;

public class ContactMessageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AdminReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? RepliedByUserId { get; set; }
    public string? RepliedByUserName { get; set; }
}

public class CreateContactMessageRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }
    
    [Required(ErrorMessage = "Subject is required")]
    [MaxLength(255, ErrorMessage = "Subject cannot exceed 255 characters")]
    public string Subject { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Message is required")]
    [MaxLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
    public string Message { get; set; } = string.Empty;
}

public class ReplyContactMessageRequestDto
{
    [Required(ErrorMessage = "Admin reply is required")]
    [MaxLength(2000, ErrorMessage = "Admin reply cannot exceed 2000 characters")]
    public string AdminReply { get; set; } = string.Empty;
}

public class UpdateContactMessageStatusRequestDto
{
    [Required(ErrorMessage = "Status is required")]
    [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string Status { get; set; } = string.Empty;
}
