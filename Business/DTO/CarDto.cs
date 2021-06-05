using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class CarDto
    {
        public int Id { get; set; }

        public UserDto Owner { get; set; }

        public ModelDto Model { get; set; }
    }
}
