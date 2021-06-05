using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OrderService
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ServiceId { get; set; }

        public Service Service { get; set; }

        public double Price{ get; set; }

        public Mechanic Mechanic { get; set; }

        public OrderServiceStatus OrderServiceStatus { get; set; }
    }
}
