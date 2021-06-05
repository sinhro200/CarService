using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public Car Car { get; set; }

        public DateTime DateTime { get; set; }

        public List<Service> Services { get; set; }

        public List<OrderService> OrderServices { get; set; }
    }
}
