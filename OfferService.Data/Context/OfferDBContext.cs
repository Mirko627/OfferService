using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfferService.Repository.Entities;
using OfferService.Shared.enums;

namespace OfferService.Data.Context
{
    public class OfferDBContext : DbContext
    {
        public DbSet<Offer> Offers { get; set; }

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
        }
    }
}
