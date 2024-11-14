using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Handlers;
using NotificationService.Application.Interfaces;
using NotificationService.Controller;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<NotificationHandler>();
builder.Services.AddScoped<NotificationMQService>();
builder.Services.AddScoped<NotificationController>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddDbContext<NotificationDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("NotificationServiceDatabase")));

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDBContext>();

    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
    {
        await dbContext.Database.MigrateAsync();
    }

    // Resolve services
    var notificationHandler = scope.ServiceProvider.GetRequiredService<NotificationHandler>();
    var notificationMQService = scope.ServiceProvider.GetRequiredService<NotificationMQService>();

    // Start listening
    notificationMQService.StartListening();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();