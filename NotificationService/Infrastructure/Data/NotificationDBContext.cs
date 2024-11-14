using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Data
{
    public class NotificationDBContext : DbContext
    {
        public NotificationDBContext(DbContextOptions<NotificationDBContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerEmail).HasMaxLength(100);
                entity.Property(e => e.CustomerPhone).HasMaxLength(20);
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.SentDate).HasDefaultValueSql("NOW()");
            });
        }
    }
}
