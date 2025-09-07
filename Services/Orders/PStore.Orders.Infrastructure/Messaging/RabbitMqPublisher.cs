using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace PStore.Orders.Infrastructure.Messaging;

public class RabbitMqPublisher : IDisposable {
    private readonly IConnection _connection;
    private readonly RabbitMQ.Client.IModel _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger) {
        _logger = logger;
        var factory = new ConnectionFactory {
            HostName = "localhost", 
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "orders", type: ExchangeType.Fanout, durable: true);
    }

    public void Publish(string eventType, object message) {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(exchange: "orders",
                              routingKey: "",
                              basicProperties: null,
                              body: body);
        _logger.LogInformation("Evento {EventType} publicado no RabbitMQ", eventType);
    }

    public void Dispose() {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
