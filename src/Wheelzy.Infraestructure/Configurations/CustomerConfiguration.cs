using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> e)
        {
            e.ToTable("Customer");
            e.HasKey(x => x.CustomerId);

            e.Property(x => x.Name).IsRequired().HasMaxLength(200).IsUnicode(false);
            e.Property(x => x.ZipCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(5)
                .IsUnicode(false);

            e.Property(x => x.Balance).HasColumnType("money").HasDefaultValue(0);
            e.HasIndex(x => x.Name).IsUnique();

            e.HasOne(x => x.ZipCodeRef)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.ZipCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
