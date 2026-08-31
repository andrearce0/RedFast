using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RedFast.Modules.Core.Infrastructure.Messaging;

public class RabbitMqMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly string _hostname = "localhost";
    private readonly string _exchangeName = "redfast-exchange";
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublishAsync<T>(T message, string routeKey)
    {
        var json = JsonSerializer.Serialize(message);

        await PublishJsonAsync(json, routeKey);
    }

    public async Task PublishJsonAsync(string json, string routeKey)
    {
        if (_channel is null)
        {
            var factory = new ConnectionFactory { HostName = _hostname };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
        }

        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routeKey,
            body: body
        );
    }

    public async ValueTask DisposeAsync()
    {
        if(_channel is not null) await _channel.CloseAsync();
        if(_connection is not null) await _connection.CloseAsync();
    }
}
