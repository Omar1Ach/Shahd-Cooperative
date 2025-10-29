using Microsoft.Extensions.Logging;
using ShahdCooperative.Infrastructure.MessageBroker.Events;

namespace ShahdCooperative.Infrastructure.MessageBroker.Handlers;

public class UserLoggedInEventHandler : IEventHandler<UserLoggedInEvent>
{
    private readonly ILogger<UserLoggedInEventHandler> _logger;

    public UserLoggedInEventHandler(ILogger<UserLoggedInEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(UserLoggedInEvent @event)
    {
        _logger.LogInformation(
            "User {UserId} ({Email}) logged in from IP {IpAddress} at {LoggedInAt}",
            @event.UserId,
            @event.Email,
            @event.IpAddress ?? "unknown",
            @event.LoggedInAt);

        // Future: Update last login timestamp, track login analytics, etc.

        return Task.CompletedTask;
    }
}
