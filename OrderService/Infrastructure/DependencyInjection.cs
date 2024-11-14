using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NpgsqlTypes;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace OrderService.Infrastructure
{
    public static class DependencyInjectionRegister
    {
        public static IServiceCollection OrderDependencies(this IServiceCollection services,
          IConfiguration configration,
          IHostBuilder hostBuilder)
        {
            services.AddControllers();
            services.AddScoped<IOrderRepository, OrderRepository>(); // Assuming OrderRepository is the implementation of IOrderRepository
            services.AddScoped<OrderMQService>();
            services.AddScoped<Application.Services.OrderService>();
            services.AddDbContext<OrderDBContext>(options =>
              options.UseNpgsql(configration.GetConnectionString("OrderServiceDatabase")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            hostBuilder.UseSerilog((context, services, configuration) => configuration
              .ReadFrom.Configuration(context.Configuration)
              .ReadFrom.Services(services)
              .Enrich.FromLogContext()
              .WriteTo.Console()
              .WriteTo.PostgreSQL(
                connectionString: context.Configuration.GetConnectionString("OrderServiceDatabase"),
                tableName: "LogEntries",
                needAutoCreateTable: true,
                columnOptions: new Dictionary<string, ColumnWriterBase> {
            { "Message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "MessageTemplate", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "Level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "TimeStamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
            { "Exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "Properties", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            { "LogEvent", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) }
                }));

            return services;
        }
    }
}