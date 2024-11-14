using Microsoft.EntityFrameworkCore;
using Serilog;
using StockService.Application.Handlers;
using StockService.Application.Interfaces;
using StockService.Application.Services;
using StockService.Infrastructure.Data;
using StockService.Infrastructure.Repositories;
using StockService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IStockRepository, StockRepository>(); // Register IStockRepository as scoped
builder.Services.AddScoped<StockHandler>(); // Change from AddSingleton to AddScoped
builder.Services.AddSingleton<StockMQService>();
builder.Services.AddHostedService<StockMQBackgroundService>();
builder.Services.AddDbContext<StockDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("StockServiceDatabase")));

// Add Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StockDBContext>();

    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
    {
        await dbContext.Database.MigrateAsync();
    }

    // Resolve and start StockMQService
    var stockMQService = scope.ServiceProvider.GetRequiredService<StockMQService>();
    stockMQService.StartListening(message =>
    {
        // Handle the received message
        Console.WriteLine($"Received message: {message}");
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.RunAsync();