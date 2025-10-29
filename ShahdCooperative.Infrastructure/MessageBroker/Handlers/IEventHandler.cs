namespace ShahdCooperative.Infrastructure.MessageBroker.Handlers;

public interface IEventHandler<in TEvent>
{
    Task HandleAsync(TEvent @event);
}
