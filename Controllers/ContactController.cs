using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstabraqTourismAPI.Services;
using EstabraqTourismAPI.DTOs.Contact;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IContactService contactService, ILogger<ContactController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    /// <summary>
    /// Submit contact message (Public)
    /// </summary>
    /// <param name="request">Contact message data</param>
    /// <returns>Success message</returns>
    [HttpPost("message")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    public async Task<ActionResult<ApiResponse<string>>> SubmitContactMessage([FromBody] ContactMessageRequestDto request)
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

            var result = await _contactService.SubmitContactMessageAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SubmitContactMessage endpoint");
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get all contact messages (Admin only)
    /// </summary>
    /// <param name="request">Pagination and filter parameters</param>
    /// <returns>Paginated list of contact messages</returns>
    [HttpGet("messages")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ContactMessageDto>>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<PagedResult<ContactMessageDto>>>> GetContactMessages([FromQuery] GetContactMessagesRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<PagedResult<ContactMessageDto>>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _contactService.GetContactMessagesAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetContactMessages endpoint");
            return StatusCode(500, ApiResponse<PagedResult<ContactMessageDto>>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get contact message by ID (Admin only)
    /// </summary>
    /// <param name="id">Contact message ID</param>
    /// <returns>Contact message information</returns>
    [HttpGet("messages/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> GetContactMessage(int id)
    {
        try
        {
            var result = await _contactService.GetContactMessageByIdAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetContactMessage endpoint for ID {MessageId}", id);
            return StatusCode(500, ApiResponse<ContactMessageDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Mark contact message as read (Admin only)
    /// </summary>
    /// <param name="id">Contact message ID</param>
    /// <returns>Updated contact message</returns>
    [HttpPatch("messages/{id}/mark-read")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageDto>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<ContactMessageDto>>> MarkAsRead(int id)
    {
        try
        {
            var result = await _contactService.MarkAsReadAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MarkAsRead endpoint for ID {MessageId}", id);
            return StatusCode(500, ApiResponse<ContactMessageDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Reply to contact message (Admin only)
    /// </summary>
    /// <param name="id">Contact message ID</param>
    /// <param name="request">Reply data</param>
    /// <returns>Success message</returns>
    [HttpPost("messages/{id}/reply")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 400)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<string>>> ReplyToMessage(int id, [FromBody] ReplyToContactMessageRequestDto request)
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

            var result = await _contactService.ReplyToMessageAsync(id, request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ReplyToMessage endpoint for ID {MessageId}", id);
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Delete contact message (Admin only)
    /// </summary>
    /// <param name="id">Contact message ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("messages/{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<string>), 404)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<string>>> DeleteContactMessage(int id)
    {
        try
        {
            var result = await _contactService.DeleteContactMessageAsync(id);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteContactMessage endpoint for ID {MessageId}", id);
            return StatusCode(500, ApiResponse<string>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get contact information (Public)
    /// </summary>
    /// <returns>Contact information</returns>
    [HttpGet("info")]
    [ProducesResponseType(typeof(ApiResponse<ContactInfoDto>), 200)]
    public async Task<ActionResult<ApiResponse<ContactInfoDto>>> GetContactInfo()
    {
        try
        {
            var result = await _contactService.GetContactInfoAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetContactInfo endpoint");
            return StatusCode(500, ApiResponse<ContactInfoDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Update contact information (Admin only)
    /// </summary>
    /// <param name="request">Updated contact information</param>
    /// <returns>Updated contact information</returns>
    [HttpPut("info")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ContactInfoDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ContactInfoDto>), 400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<ContactInfoDto>>> UpdateContactInfo([FromBody] UpdateContactInfoRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<ContactInfoDto>.FailureResult(
                    "Validation failed", errors));
            }

            var result = await _contactService.UpdateContactInfoAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateContactInfo endpoint");
            return StatusCode(500, ApiResponse<ContactInfoDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }

    /// <summary>
    /// Get contact message statistics (Admin only)
    /// </summary>
    /// <returns>Contact message statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ContactStatisticsDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<ContactStatisticsDto>>> GetContactStatistics()
    {
        try
        {
            var result = await _contactService.GetContactStatisticsAsync();
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetContactStatistics endpoint");
            return StatusCode(500, ApiResponse<ContactStatisticsDto>.FailureResult(
                "An error occurred while processing your request"));
        }
    }
}
