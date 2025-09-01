using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelzy.Application.Models
{
    public class CustomerCar
    {
        public int CustomerCarId { get; set; } 
        public int CustomerId { get; set; }
        public int CarId { get; set; }

        public Customer Customer { get; set; } = null!;
        public Car Car { get; set; } = null!;

    }
}
