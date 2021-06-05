using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Service
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public List<Mechanic> Mechanics { get; set; } = new List<Mechanic>();

        public List<Order> Orders { get; set; }

        public List<OrderService> OrderServices { get; set; }
    }
}
