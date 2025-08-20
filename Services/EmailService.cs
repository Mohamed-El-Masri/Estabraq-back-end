using System.Net;
using System.Net.Mail;
using EstabraqTourismAPI.Configuration;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Services;

public interface IEmailService
{
    Task<ApiResponse<string>> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<ApiResponse<string>> SendBookingConfirmationAsync(string to, string customerName, string bookingReference, string tripTitle);
    Task<ApiResponse<string>> SendBookingStatusUpdateAsync(string to, string customerName, string bookingReference, string status, string? adminNotes = null);
    Task<ApiResponse<string>> SendContactReplyAsync(string to, string customerName, string originalSubject, string replyMessage);
    Task<ApiResponse<string>> SendWelcomeEmailAsync(string to, string customerName);
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        EmailSettings emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            if (!_emailSettings.EnableEmail)
            {
                _logger.LogInformation("Email sending is disabled. Email to {To} with subject '{Subject}' not sent", to, subject);
                return ApiResponse<string>.SuccessResult("", "Email sending is disabled");
            }

            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
            client.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword);
            client.EnableSsl = _emailSettings.SmtpUseSsl;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {To} with subject '{Subject}'", to, subject);
            return ApiResponse<string>.SuccessResult("", "Email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To} with subject '{Subject}'", to, subject);
            return ApiResponse<string>.FailureResult("Failed to send email");
        }
    }

    public async Task<ApiResponse<string>> SendBookingConfirmationAsync(string to, string customerName, string bookingReference, string tripTitle)
    {
        try
        {
            var subject = $"Booking Confirmation - {bookingReference}";
            var body = $@"
                <html>
                <body dir='rtl' style='font-family: Arial, sans-serif; direction: rtl; text-align: right;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <h2 style='color: #2c5aa0; text-align: center;'>تأكيد الحجز</h2>
                        
                        <p>عزيزي/عزيزتي {customerName}،</p>
                        
                        <p>نشكركم لاختياركم شركة استبرق للسياحة. تم استلام طلب الحجز الخاص بكم بنجاح.</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='color: #2c5aa0; margin-top: 0;'>تفاصيل الحجز:</h3>
                            <p><strong>رقم الحجز:</strong> {bookingReference}</p>
                            <p><strong>اسم الرحلة:</strong> {tripTitle}</p>
                            <p><strong>حالة الحجز:</strong> قيد المراجعة</p>
                        </div>
                        
                        <p>سيتم مراجعة حجزكم والتواصل معكم خلال 24 ساعة لتأكيد التفاصيل النهائية.</p>
                        
                        <p>إذا كانت لديكم أي استفسارات، يرجى التواصل معنا:</p>
                        <ul>
                            <li>الهاتف: +964 770 123 4567</li>
                            <li>البريد الإلكتروني: info@estabraqtourism.com</li>
                        </ul>
                        
                        <p style='text-align: center; margin-top: 30px; color: #666;'>
                            شكراً لاختياركم شركة استبرق للسياحة<br>
                            نتطلع لخدمتكم
                        </p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking confirmation email");
            return ApiResponse<string>.FailureResult("Failed to send booking confirmation email");
        }
    }

    public async Task<ApiResponse<string>> SendBookingStatusUpdateAsync(string to, string customerName, string bookingReference, string status, string? adminNotes = null)
    {
        try
        {
            var statusArabic = status switch
            {
                "Confirmed" => "مؤكد",
                "Cancelled" => "ملغي",
                "Completed" => "مكتمل",
                _ => "قيد المراجعة"
            };

            var subject = $"تحديث حالة الحجز - {bookingReference}";
            var body = $@"
                <html>
                <body dir='rtl' style='font-family: Arial, sans-serif; direction: rtl; text-align: right;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <h2 style='color: #2c5aa0; text-align: center;'>تحديث حالة الحجز</h2>
                        
                        <p>عزيزي/عزيزتي {customerName}،</p>
                        
                        <p>نود إعلامكم بتحديث حالة حجزكم:</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p><strong>رقم الحجز:</strong> {bookingReference}</p>
                            <p><strong>الحالة الجديدة:</strong> <span style='color: {(status == "Confirmed" ? "#28a745" : status == "Cancelled" ? "#dc3545" : "#2c5aa0")};'>{statusArabic}</span></p>
                            {(string.IsNullOrEmpty(adminNotes) ? "" : $"<p><strong>ملاحظات:</strong> {adminNotes}</p>")}
                        </div>
                        
                        <p>إذا كانت لديكم أي استفسارات، يرجى التواصل معنا:</p>
                        <ul>
                            <li>الهاتف: +964 770 123 4567</li>
                            <li>البريد الإلكتروني: info@estabraqtourism.com</li>
                        </ul>
                        
                        <p style='text-align: center; margin-top: 30px; color: #666;'>
                            شكراً لاختياركم شركة استبرق للسياحة
                        </p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking status update email");
            return ApiResponse<string>.FailureResult("Failed to send booking status update email");
        }
    }

    public async Task<ApiResponse<string>> SendContactReplyAsync(string to, string customerName, string originalSubject, string replyMessage)
    {
        try
        {
            var subject = $"رد على استفساركم - {originalSubject}";
            var body = $@"
                <html>
                <body dir='rtl' style='font-family: Arial, sans-serif; direction: rtl; text-align: right;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <h2 style='color: #2c5aa0; text-align: center;'>رد على استفساركم</h2>
                        
                        <p>عزيزي/عزيزتي {customerName}،</p>
                        
                        <p>نشكركم لتواصلكم معنا. فيما يلي ردنا على استفساركم:</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <p><strong>الموضوع الأصلي:</strong> {originalSubject}</p>
                        </div>
                        
                        <div style='background-color: #e7f3ff; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='color: #2c5aa0; margin-top: 0;'>ردنا:</h3>
                            <p>{replyMessage}</p>
                        </div>
                        
                        <p>إذا كانت لديكم أي استفسارات إضافية، لا تترددوا في التواصل معنا:</p>
                        <ul>
                            <li>الهاتف: +964 770 123 4567</li>
                            <li>البريد الإلكتروني: info@estabraqtourism.com</li>
                        </ul>
                        
                        <p style='text-align: center; margin-top: 30px; color: #666;'>
                            شكراً لاختياركم شركة استبرق للسياحة<br>
                            نحن في خدمتكم دائماً
                        </p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending contact reply email");
            return ApiResponse<string>.FailureResult("Failed to send contact reply email");
        }
    }

    public async Task<ApiResponse<string>> SendWelcomeEmailAsync(string to, string customerName)
    {
        try
        {
            var subject = "مرحباً بكم في شركة استبرق للسياحة";
            var body = $@"
                <html>
                <body dir='rtl' style='font-family: Arial, sans-serif; direction: rtl; text-align: right;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <h2 style='color: #2c5aa0; text-align: center;'>مرحباً بكم في شركة استبرق للسياحة</h2>
                        
                        <p>عزيزي/عزيزتي {customerName}،</p>
                        
                        <p>مرحباً بكم في شركة استبرق للسياحة! نحن سعداء بانضمامكم إلى عائلتنا.</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                            <h3 style='color: #2c5aa0; margin-top: 0;'>ما يمكنكم فعله الآن:</h3>
                            <ul>
                                <li>تصفح رحلاتنا المتنوعة</li>
                                <li>حجز الرحلات المفضلة لديكم</li>
                                <li>متابعة حالة حجوزاتكم</li>
                                <li>التواصل معنا لأي استفسارات</li>
                            </ul>
                        </div>
                        
                        <p>نحن ملتزمون بتقديم أفضل الخدمات السياحية لكم ونتطلع لخدمتكم.</p>
                        
                        <p>للتواصل معنا:</p>
                        <ul>
                            <li>الهاتف: +964 770 123 4567</li>
                            <li>البريد الإلكتروني: info@estabraqtourism.com</li>
                            <li>الموقع الإلكتروني: https://estabraqtourism.com</li>
                        </ul>
                        
                        <p style='text-align: center; margin-top: 30px; color: #666;'>
                            شكراً لاختياركم شركة استبرق للسياحة<br>
                            نتطلع لرحلة ممتعة معكم
                        </p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending welcome email");
            return ApiResponse<string>.FailureResult("Failed to send welcome email");
        }
    }
}
