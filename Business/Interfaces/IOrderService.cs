using Business.DTO;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IOrderService : IDefaultService<OrderDto>
    {
        List<OrderDto> AllOrdersForCar(int carId);
        List<OrderDto> OrdersWithFilter(OrderFilterDto orderFilterDto);
        List<OrderDto> OrdersWithFilterOrdeing(OrderFilterDto orderFilterDto, int orderCode, bool isAsc);
        List<FullServiceDto> ServicesForOrder(int orderId);
        MemoryStream ToXml(List<OrderDto> orders, OrderFilterDto orderFilter);
        void TryCloseOrder(int orderId);
    }
}
