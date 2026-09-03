using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfferService.Repository.Entities;
using OfferService.Shared.enums;

namespace OfferService.Data.Context
{
    public class OfferDBContext : DbContext
    {
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }
        public OfferDBContext(DbContextOptions<OfferDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Offer>(entity =>
            {
                entity.Property(u => u.Status)
                    .HasConversion<string>()
                    .IsRequired()
                    .HasDefaultValue(OfferStatus.Pending);

                entity.ToTable(t => t.HasCheckConstraint(
                   name: "Ck_Offer_Status",
                   sql: "Status IN ('Pending', 'Accepted', 'Rejected', 'Expired')"
               ));

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            builder.Entity<OutboxEvent>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Payload)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.PublishedAt)
                    .IsRequired(false);

                entity.HasIndex(e => new { e.PublishedAt, e.CreatedAt });
            });
        }
    }
}
