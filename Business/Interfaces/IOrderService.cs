using Business.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IOrderService : IDefaultService<OrderDto>
    {
        List<OrderDto> AllOrdersForCar(int carId);
        List<FullServiceDto> ServicesForOrder(int orderId);
        void TryCloseOrder(int orderId);
    }
}
