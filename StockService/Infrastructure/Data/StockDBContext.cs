using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entites;
using StockService.Domain.Entities;

namespace StockService.Infrastructure.Data
{
    public class StockDBContext : DbContext
    {
        public StockDBContext(DbContextOptions<StockDBContext> options) : base(options) { }

        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductId)
                      .IsRequired();
                entity.Property(e => e.OrderId)
                      .IsRequired();
                entity.Property(e => e.MovementDate)
                      .IsRequired().HasDefaultValueSql("now()");
                entity.Property(e => e.Quantity)
                      .IsRequired();
                entity.Property(e => e.MovementType)
                      .IsRequired()
                      .HasMaxLength(3);
            });

            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.Exception);
                entity.Property(e => e.Properties);
                entity.Property(e => e.LogEvent);
            });
        }
    }
}