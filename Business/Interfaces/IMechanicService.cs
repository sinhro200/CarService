using Business.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IMechanicService : IDefaultService<MechanicDto>
    {
        List<FullServiceDto> AllServicesByMechanic(int mechanicId);
    }
}
