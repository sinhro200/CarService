using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class OrderServiceStatus
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public List<OrderService> OrderServices { get; set; }
    }
}
