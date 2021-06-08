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

        public int? MechanicId { get; set; }
        public Mechanic? Mechanic { get; set; }

        public int OrderServiceStatusId { get; set; } = 1;
        public OrderServiceStatus OrderServiceStatus { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(OrderId)}={OrderId.ToString()}, {nameof(Order)}={Order}, {nameof(ServiceId)}={ServiceId.ToString()}, {nameof(Service)}={Service}, {nameof(Price)}={Price.ToString()}, {nameof(MechanicId)}={MechanicId.ToString()}, {nameof(Mechanic)}={Mechanic}, {nameof(OrderServiceStatusId)}={OrderServiceStatusId.ToString()}, {nameof(OrderServiceStatus)}={OrderServiceStatus}}}";
        }
    }
}
