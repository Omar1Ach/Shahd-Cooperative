using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<bool> ValidateTokenAsync(string token);
    Task<string?> GetUserIdFromTokenAsync(string token);
}
