using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> b)
        {
            b.ToTable("OrderStatusHistory");
            b.HasKey(x => x.OrderStatusHistoryId);

            b.Property(x => x.CreatedAt).HasDefaultValueSql("sysutcdatetime()");

            b.HasOne(x => x.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Status)
                .WithMany(s => s.Histories)
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // CHECK (StatusId <> 3 OR StatusDate IS NOT NULL)
            b.ToTable(t => t.HasCheckConstraint("CK_CaseStatusHistory_PickedUpDate",
                "([StatusId] <> 3 OR [StatusDate] IS NOT NULL)"));
        }
    }
}
