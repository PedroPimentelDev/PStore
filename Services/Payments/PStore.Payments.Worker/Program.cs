await Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) => {
        services.AddHostedService<PaymentsWorker>();
        services.AddSingleton(new RabbitOptions(ctx.Configuration.GetSection("Rabbit")));
    })
    .RunConsoleAsync();