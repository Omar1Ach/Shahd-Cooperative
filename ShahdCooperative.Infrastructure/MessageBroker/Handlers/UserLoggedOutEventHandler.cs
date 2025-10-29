using Microsoft.Extensions.Logging;
using ShahdCooperative.Infrastructure.MessageBroker.Events;

namespace ShahdCooperative.Infrastructure.MessageBroker.Handlers;

public class UserLoggedOutEventHandler : IEventHandler<UserLoggedOutEvent>
{
    private readonly ILogger<UserLoggedOutEventHandler> _logger;

    public UserLoggedOutEventHandler(ILogger<UserLoggedOutEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(UserLoggedOutEvent @event)
    {
        _logger.LogInformation(
            "User {UserId} logged out at {LoggedOutAt}",
            @event.UserId,
            @event.LoggedOutAt);

        // Future: Track logout analytics, session duration, etc.

        return Task.CompletedTask;
    }
}
