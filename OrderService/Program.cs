using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.OrderDependencies(builder.Configuration, builder.Host);
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDBContext>();

    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Any())
    {
        await dbContext.Database.MigrateAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.MapControllers();

await app.RunAsync();