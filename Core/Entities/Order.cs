using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int CarId { get; set; }
        [ForeignKey("CarId")]
        public Car Car { get; set; }

        public DateTime OpenDateTime { get; set; }

        public DateTime? CloseDateTime { get; set; }

        public bool IsClosed { get; set; } = false;

        public List<Service> Services { get; set; }

        public List<OrderService> OrderServices { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(CarId)}={CarId.ToString()}, {nameof(Car)}={Car}, {nameof(OpenDateTime)}={OpenDateTime.ToString()}, {nameof(CloseDateTime)}={CloseDateTime.ToString()}, {nameof(IsClosed)}={IsClosed.ToString()}, {nameof(Services)}={Services}, {nameof(OrderServices)}={OrderServices}}}";
        }
    }
}
