using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> b)
        {
            b.ToTable("Invoice");
            b.HasKey(x => x.InvoiceId);

            b.Property(x => x.Amount).HasColumnType("money");
            b.Property(x => x.IssuedAt).HasDefaultValueSql("sysutcdatetime()");
            b.Property(x => x.Currency).HasMaxLength(3).IsRequired().HasDefaultValue("USD");
            b.Property(x => x.ExternalNumber).HasMaxLength(50);
            b.Property(x => x.Notes).HasMaxLength(500);

            b.HasOne(x => x.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Order)
                .WithMany(o => o.Invoices)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.CustomerId, x.IssuedAt }).HasDatabaseName("IX_Invoice_CustomerId_IssuedAt");
            b.HasIndex(x => x.OrderId).HasDatabaseName("IX_Invoice_OrderId");

            // CHECK ((IsPaid=0 AND PaidAt IS NULL) OR (IsPaid=1 AND PaidAt IS NOT NULL))
            b.ToTable(t => t.HasCheckConstraint("CK_Invoice_PaidDate",
                "([IsPaid]=(0) AND [PaidAt] IS NULL) OR ([IsPaid]=(1) AND [PaidAt] IS NOT NULL)"));
        }
    }
}
