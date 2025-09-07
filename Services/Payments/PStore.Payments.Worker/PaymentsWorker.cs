using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;



public sealed class RabbitOptions {
    public string Host { get; }
    public int Port { get; }
    public string User { get; }
    public string Pass { get; }
    public string OrdersExchange { get; }
    public string PaymentsExchange { get; }
    public string Queue { get; }

    public RabbitOptions(IConfiguration cfg) {
        Host = cfg["Host"]!;
        Port = int.Parse(cfg["Port"]!);
        User = cfg["User"]!;
        Pass = cfg["Pass"]!;
        OrdersExchange = cfg["OrdersExchange"]!;
        PaymentsExchange = cfg["PaymentsExchange"]!;
        Queue = cfg["Queue"]!;
    }
}

public sealed class PaymentsWorker : BackgroundService {
    private readonly ILogger<PaymentsWorker> _log;
    private readonly RabbitOptions _opt;
    private IConnection? _conn;
    private IModel? _ch;

    public PaymentsWorker(ILogger<PaymentsWorker> log, RabbitOptions opt) {
        _log = log;
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

        _conn = factory.CreateConnection("pstore-payments");
        _ch = _conn.CreateModel();

        _ch.ExchangeDeclare(_opt.OrdersExchange, ExchangeType.Fanout, durable: true);
        _ch.QueueDeclare(_opt.Queue, durable: true, exclusive: false, autoDelete: false);
        _ch.QueueBind(_opt.Queue, _opt.OrdersExchange, routingKey: "");

        _ch.ExchangeDeclare(_opt.PaymentsExchange, ExchangeType.Topic, durable: true);

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.Received += async (s, ea) => {
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            _log.LogInformation("Recebido OrderCreated: {json}", json);

            var ok = Random.Shared.NextDouble() < 0.9;

            var confirmed = new {
                Type = ok ? "PaymentConfirmed.v1" : "PaymentRejected.v1",
                Data = new {
                    OrderId = TryGetOrderId(json),
                    ProcessedAt = DateTime.UtcNow
                }
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(confirmed));

            var rk = ok ? "payments.confirmed.v1" : "payments.rejected.v1";
            _ch.BasicPublish(exchange: _opt.PaymentsExchange, routingKey: rk, basicProperties: null, body: body);

            _ch.BasicAck(ea.DeliveryTag, multiple: false);
            await Task.CompletedTask;
        };

        _ch.BasicQos(0, 10, false);
        _ch.BasicConsume(_opt.Queue, autoAck: false, consumer);

        _log.LogInformation("PaymentsWorker consumindo {queue}", _opt.Queue);
        return Task.CompletedTask;
    }

    private static Guid TryGetOrderId(string json) {
        try {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("OrderId", out var idProp) &&
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
