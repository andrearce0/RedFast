namespace RedFast.Modules.Core.Infrastructure.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string routingKey);
}
