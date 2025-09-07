using PStore.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PStore.Catalog.Domain;
using PStore.Catalog.Api;
using PStore.Catalog.Application.UseCases.CreateProduct;
using PStore.Catalog.Application.UseCases.GetAllProductsBySku;
using PStore.Catalog.Application.UseCases.GetAllProducts;
using PStore.Catalog.Application.UseCases.GetProductById;
using PStore.Catalog.Application.UseCases.UpdateProduct;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHostedService<SeedHostedService>();

builder.Services.AddScoped<ICreateProductUseCase, CreateProductUseCase>();
builder.Services.AddScoped<IGetAllProductsUseCase, GetAllProductsUseCase>();
builder.Services.AddScoped<IGetAllProductsBySkuUseCase, GetAllProductsBySkuUseCase>();
builder.Services.AddScoped<IGetProductByIdUseCase, GetProductByIdUseCase>();
builder.Services.AddScoped<IUpdateProductUseCase, UpdateProductUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/", () => "Bem-vindo à API!");

app.Run();