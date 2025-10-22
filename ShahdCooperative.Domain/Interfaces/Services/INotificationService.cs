using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Services;

public interface INotificationService
{
    Task SendEmailAsync(string recipient, string subject, string body);
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendPushNotificationAsync(string userId, string message);
}
