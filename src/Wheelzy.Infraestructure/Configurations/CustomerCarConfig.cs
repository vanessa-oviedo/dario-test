using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class CustomerCarConfiguration : IEntityTypeConfiguration<CustomerCar>
    {
        public void Configure(EntityTypeBuilder<CustomerCar> e)
        {
            e.ToTable("CustomerCars");
            e.HasKey(x => x.CustomerCarId);

            e.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerCars)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Car)
                .WithMany(x => x.CustomerCars)
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
