using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }

        public CarDto Car { get; set; }

        public DateTime DateTime { get; set; }

        public List<FullServiceDto> Services { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(Car)}={Car}, {nameof(DateTime)}={DateTime.ToString()}, {nameof(Services)}={Services}}}";
        }
    }
}
