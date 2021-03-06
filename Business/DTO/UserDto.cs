using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<CarDto> Cars { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(Name)}={Name}}}";
        }
    }
}
