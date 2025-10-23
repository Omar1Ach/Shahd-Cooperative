using Microsoft.Extensions.Configuration;
using ShahdCooperative.Domain.Interfaces.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ShahdCooperative.Infrastructure.ExternalServices;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _authServiceUrl;

    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _authServiceUrl = configuration["ExternalServices:AuthService:BaseUrl"]
            ?? throw new InvalidOperationException("Auth service URL not configured");
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_authServiceUrl}/validate");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_authServiceUrl}/user-info");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
            return userInfo?.UserId;
        }
        catch
        {
            return null;
        }
    }

    private class UserInfoResponse
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
    }
}
