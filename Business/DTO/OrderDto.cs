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

        public DateTime CreateDateTime { get; set; }

        public DateTime? ClosedDateTime { get; set; }

        public bool IsClosed { get; set; } = false;

        public List<FullServiceDto> Services { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(Car)}={Car}, {nameof(CreateDateTime)}={CreateDateTime.ToString()}, {nameof(Services)}={Services}}}";
        }
    }
}
