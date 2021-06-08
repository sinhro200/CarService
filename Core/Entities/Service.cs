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

        public double DefaultPrice{ get; set; }

        public List<Mechanic> Mechanics { get; set; } = new List<Mechanic>();

        public List<Order> Orders { get; set; }

        public List<OrderService> OrderServices { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(Title)}={Title}, {nameof(DefaultPrice)}={DefaultPrice.ToString()}, {nameof(Mechanics)}={Mechanics}, {nameof(Orders)}={Orders}, {nameof(OrderServices)}={OrderServices}}}";
        }
    }
}
