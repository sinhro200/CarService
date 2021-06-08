using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class MechanicDto: UserDto
    {
        public List<ServiceDto> Services { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Services)}={Services}, {nameof(Id)}={Id.ToString()}, {nameof(Name)}={Name}, {nameof(Cars)}={Cars}}}";
        }
    }
}
