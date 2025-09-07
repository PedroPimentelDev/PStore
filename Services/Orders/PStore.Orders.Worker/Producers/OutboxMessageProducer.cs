using PStore.Orders.Domain;
using PStore.Orders.Infrastructure;
using PStore.Orders.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace PStore.Orders.Api;

public class OutboxMessageProducer : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxMessageProducer> _logger;
    private readonly RabbitMqPublisher _publisher;

    public OutboxMessageProducer(IServiceScopeFactory scopeFactory,
                           ILogger<OutboxMessageProducer> logger,
                           RabbitMqPublisher publisher) {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var messages = db.OutboxMessages
                .Where(m => m.ProcessedOnUtc == null)
                .ToList();

            foreach (var msg in messages) {
                _logger.LogInformation("Publicando evento {Type} no RabbitMQ", msg.Type);

                var msgContent = JsonSerializer.Deserialize<object>(msg.Content)!;
                _publisher.Publish(msg.Type, msgContent);

                msg.ProcessedOnUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
            await Task.Delay(5000, stoppingToken);
        }
    }
}
