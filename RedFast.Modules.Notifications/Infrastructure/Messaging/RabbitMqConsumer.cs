using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RedFast.Modules.Notifications.Infrastructure.Messaging;

public class RabbitMqConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly string _hostName = "localhost";
    private readonly string _exchangeName = "redfast-exchange";
    private readonly string _queueName = "notifications-package-status-queue";
    private readonly string _routingKey = "package.status.changed";

    public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _hostName };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
        
        await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

        await channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey);

        _logger.LogInformation(" [*] Módulo de Notificações aguardando mensagens na fila {QueueName}...", _queueName);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var @event = JsonSerializer.Deserialize<PackageStatusChangedEvent>(message, options);


                _logger.LogInformation(
                    "[x] PUSH ENVIADO: Cliente notificado que o pacote {Id} mudou de {Old} para {New}",
                    @event?.PackageId, @event?.OldStatus, @event?.NewStatus);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "[!] Falha ao processar e desserializar a mensagem do RabbitMQ.");

                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }
}
