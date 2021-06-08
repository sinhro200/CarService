using Business.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IServiceService : IDefaultService<ServiceDto>
    {
        List<ServiceDto> AllServicesForCar(int carId);
    }
}
