using Microsoft.EntityFrameworkCore;
using PStore.Orders.Infrastructure;
using PStore.Orders.Infrastructure.Messaging;
using PStore.Orders.Application.UseCases.CreateOrder;
using PStore.Orders.Application.UseCases.GetOrder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrdersDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IOrdersDbContext>(provider => provider.GetRequiredService<OrdersDbContext>());

builder.Services.AddHttpClient("catalog", c => {
    c.BaseAddress = new Uri(builder.Configuration["Catalog:BaseUrl"]!);
});

builder.Services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
builder.Services.AddScoped<IGetOrderUseCase, GetOrderUseCase>();

builder.Services.AddSingleton<RabbitMqPublisher>();

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
