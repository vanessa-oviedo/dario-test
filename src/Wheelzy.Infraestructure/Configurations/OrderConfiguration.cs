using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.ToTable("Order");
            b.HasKey(x => x.OrderId);

            b.Property(x => x.ZipCode).HasColumnType("char(5)").IsRequired();
            b.Property(x => x.CreatedAt).HasDefaultValueSql("sysutcdatetime()");
            b.Property(x => x.CurrentStatusChangedBy).HasMaxLength(200);

            b.HasOne(x => x.Customer)
             .WithMany(c => c.Orders)
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Car)
             .WithMany(ca => ca.Orders)
             .HasForeignKey(x => x.CarId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Zip)
             .WithMany(z => z.Orders)
             .HasForeignKey(x => x.ZipCode)
             .HasPrincipalKey(z => z.ZipCodeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.CurrentStatus)
             .WithMany(s => s.CurrentOrders)
             .HasForeignKey(x => x.CurrentStatusId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(o => o.CurrentOrderBuyerQuote)
                .WithMany()
                .HasForeignKey(o => o.CurrentOrderBuyerQuoteId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);


            // Índices que ya usás
            b.HasIndex(x => x.CurrentStatusId).HasDatabaseName("IX_Order_CurrentStatusId");
            b.HasIndex(x => new { x.CurrentOrderBuyerQuoteId, x.OrderId })
             .HasDatabaseName("IX_Order_CurrentOrderBuyerQuoteId");

            // CHECK (CurrentStatusId IS NULL OR CurrentStatusId <> 3 OR CurrentStatusDate IS NOT NULL)
            b.ToTable(t => t.HasCheckConstraint("CK_Order_PickedUpDate",
                "([CurrentStatusId] IS NULL OR [CurrentStatusId] <> 3 OR [CurrentStatusDate] IS NOT NULL)"));
        }
    }
}
