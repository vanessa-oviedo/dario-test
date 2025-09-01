using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class OrderBuyerQuoteConfiguration : IEntityTypeConfiguration<OrderBuyerQuote>
    {
        public void Configure(EntityTypeBuilder<OrderBuyerQuote> b)
        {
            b.ToTable("OrderBuyerQuote");
            b.HasKey(x => x.OrderBuyerQuoteId);

            b.Property(x => x.OrderBuyerQuoteId)
                .UseIdentityColumn()     // fuerza IDENTITY(1,1)
                .ValueGeneratedOnAdd();

            b.Property(x => x.Amount).HasColumnType("money");
            b.Property(x => x.CreatedAt).HasDefaultValueSql("sysutcdatetime()");

            b.HasOne(x => x.Order)
                .WithMany(o => o.OrderBuyerQuotes)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.BuyerZipCoverage)
                .WithMany(z => z.OrderBuyerQuotes)
                .HasForeignKey(x => x.BuyerZipCoverageId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.OrderId, x.BuyerZipCoverageId })
                .IsUnique()
                .HasDatabaseName("UQ_Quote_Per_Case_Buyer");

            // Soporte a la FK compuesta (principal key simulada con índice único)
            b.HasIndex(x => new { x.OrderBuyerQuoteId, x.OrderId })
                .IsUnique()
                .HasDatabaseName("UX_OrderBuyerQuote_Id_OrderId");
        }
    }
}
