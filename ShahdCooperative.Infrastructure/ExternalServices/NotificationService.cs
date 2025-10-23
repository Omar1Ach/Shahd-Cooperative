using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShahdCooperative.Domain.Interfaces.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ShahdCooperative.Infrastructure.ExternalServices;

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;
    private readonly string _notificationServiceUrl;

    public NotificationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<NotificationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _notificationServiceUrl = configuration["ExternalServices:NotificationService:BaseUrl"]
            ?? throw new InvalidOperationException("Notification service URL not configured");
    }

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        try
        {
            var request = new EmailRequest
            {
                To = recipient,
                Subject = subject,
                Body = body
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_notificationServiceUrl}/email",
                request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to send email to {Recipient}. Status: {StatusCode}",
                    recipient,
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Recipient}", recipient);
            throw;
        }
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var request = new SmsRequest
            {
                PhoneNumber = phoneNumber,
                Message = message
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_notificationServiceUrl}/sms",
                request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to send SMS to {PhoneNumber}. Status: {StatusCode}",
                    phoneNumber,
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", phoneNumber);
            throw;
        }
    }

    public async Task SendPushNotificationAsync(string userId, string message)
    {
        try
        {
            var request = new PushNotificationRequest
            {
                UserId = userId,
                Message = message
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_notificationServiceUrl}/push",
                request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to send push notification to user {UserId}. Status: {StatusCode}",
                    userId,
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to user {UserId}", userId);
            throw;
        }
    }

    private class EmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    private class SmsRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    private class PushNotificationRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
