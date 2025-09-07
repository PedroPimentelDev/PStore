using PStore.Orders.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PStore.Orders.Worker.Rabbit;
using PStore.Orders.Api;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) => {
        services.AddDbContext<OrdersDbContext>(opt =>
            opt.UseSqlServer(ctx.Configuration.GetConnectionString("Default")));

        services.AddSingleton(new RabbitOptions(ctx.Configuration.GetSection("Rabbit")));
        services.AddHostedService<OrdersPaymentConsumer>();
        services.AddHostedService<OutboxMessageProducer>();
    })
    .RunConsoleAsync();