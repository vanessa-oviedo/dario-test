using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class ZipCodeConfiguration : IEntityTypeConfiguration<ZipCode>
    {
        public void Configure(EntityTypeBuilder<ZipCode> b)
        {
            b.ToTable("ZipCode");
            b.HasKey(x => x.ZipCodeId);
            b.Property(x => x.ZipCodeId).HasColumnName("ZipCode").HasColumnType("char(5)");
        }
    }
}
