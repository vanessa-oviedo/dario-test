using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class CarConfig : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> b)
        {
            b.ToTable("Car");
            b.HasKey(x => x.Id);
            b.Property(x => x.Year).IsRequired();
            b.HasOne<SubModel>().WithMany().HasForeignKey(x => x.SubModelId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
