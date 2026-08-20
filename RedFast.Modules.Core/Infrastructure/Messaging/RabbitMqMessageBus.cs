using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RedFast.Modules.Core.Infrastructure.Messaging;

public class RabbitMqMessageBus : IMessageBus
{
    private readonly string _hostname = "localhost";
    private readonly string _exchangeName = "redfast-exchange";

    public async Task PublishAsync<T>(T message, string routeKey)
    {
        var factory = new ConnectionFactory { HostName = _hostname };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routeKey,
            body: body
        );
    }
}
