using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using PStore.Orders.Infrastructure;
using PStore.Orders.Domain;
using PStore.Orders.Worker.Rabbit;

public sealed class OrdersPaymentConsumer : BackgroundService {
    private readonly ILogger<OrdersPaymentConsumer> _log;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitOptions _opt;

    private IConnection? _conn;
    private IModel? _ch;

    public OrdersPaymentConsumer(ILogger<OrdersPaymentConsumer> log, IServiceScopeFactory scopeFactory, RabbitOptions opt) {
        _log = log;
        _scopeFactory = scopeFactory;
        _opt = opt;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        var factory = new ConnectionFactory {
            HostName = _opt.Host,
            Port = _opt.Port,
            UserName = _opt.User,
            Password = _opt.Pass,
            DispatchConsumersAsync = true
        };

        _conn = factory.CreateConnection("pstore-orders-consumer");
        _ch = _conn.CreateModel();

        _ch.ExchangeDeclare(_opt.Exchange, ExchangeType.Topic, durable: true);

        _ch.QueueDeclare(_opt.Queue, durable: true, exclusive: false, autoDelete: false);

        foreach (var key in _opt.Bindings)
            _ch.QueueBind(_opt.Queue, _opt.Exchange, routingKey: key);

        _ch.BasicQos(0, 10, false);

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.Received += async (s, ea) => {
            var rk = ea.RoutingKey; 
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            _log.LogInformation("Recebi {rk}: {json}", rk, json);

            Guid orderId = TryGetOrderId(json);
            if (orderId == Guid.Empty) {
                _log.LogWarning("Mensagem sem OrderId válido.");
                _ch.BasicAck(ea.DeliveryTag, false);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken: stoppingToken);
            if (order is null) {
                _log.LogWarning("Pedido {OrderId} não encontrado.", orderId);
                _ch.BasicAck(ea.DeliveryTag, false);
                return;
            }

            if (rk.StartsWith("payments.confirmed", StringComparison.OrdinalIgnoreCase))
                order.Status = OrderStatus.Paid;
            else if (rk.StartsWith("payments.rejected", StringComparison.OrdinalIgnoreCase))
                order.Status = OrderStatus.Rejected;

            await db.SaveChangesAsync(stoppingToken);
            _log.LogInformation("Pedido {OrderId} atualizado para {Status}.", orderId, order.Status);

            _ch.BasicAck(ea.DeliveryTag, multiple: false);
        };

        _ch.BasicConsume(_opt.Queue, autoAck: false, consumer);
        _log.LogInformation("OrdersPaymentConsumer ouvindo {queue}", _opt.Queue);

        return Task.CompletedTask;
    }

    private static Guid TryGetOrderId(string json) {
        try {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("Data", out var data) &&
                data.TryGetProperty("OrderId", out var idProp) &&
                Guid.TryParse(idProp.GetString(), out var id))
                return id;
        }
        catch { }
        return Guid.Empty;
    }

    public override void Dispose() {
        _ch?.Dispose();
        _conn?.Dispose();
        base.Dispose();
    }
}
