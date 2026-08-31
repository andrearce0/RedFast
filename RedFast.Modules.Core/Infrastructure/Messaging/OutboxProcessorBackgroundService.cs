using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedFast.Modules.Core.Persistence;
using System.Text.Json;

namespace RedFast.Modules.Core.Infrastructure.Messaging;

public class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;

    public OutboxProcessorBackgroundService(IServiceScopeFactory scopeFactory, 
        ILogger<OutboxProcessorBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessagesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RedFastDbContext>();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        var messages = await context.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.Error == null)
            .OrderBy(m => m.OcurredOn)
            .Take(20)
            .ToListAsync(stoppingToken);

        if (!messages.Any()) return;

        foreach (var message in messages) 
        {
            try
            {
                await messageBus.PublishJsonAsync(message.Content, "package.status.changed");

                message.MarkAsProcessed();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Erro ao processar mensagem outbox {Id}", message.Id);
                message.MarkAsFailed(ex.Message);
            }
        }

        await context.SaveChangesAsync(stoppingToken);
    }
}
