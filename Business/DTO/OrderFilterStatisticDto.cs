using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class OrderFilterStatisticDto
    {
        public int OrderCount { get; set; }

        public double Profit { get; set; }

        public Dictionary<int,double> MechanicIdToProfitMap { get; set; }

        public Dictionary<int, MechanicDto> MechanicIdToMechanicMap { get; set; }

        public Dictionary<int, double> UserIdToProfitMap { get; set; }

        public Dictionary<int, UserDto> UserIdToUserMap { get; set; }

        public Dictionary<int,int> ServiceIdToCountMap { get; set; }

        public Dictionary<int, ServiceDto> ServiceIdToServiceMap { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(OrderCount)}={OrderCount.ToString()}, {nameof(Profit)}={Profit.ToString()}, {nameof(MechanicIdToProfitMap)}={MechanicIdToProfitMap}, {nameof(UserIdToProfitMap)}={UserIdToProfitMap}, {nameof(ServiceIdToCountMap)}={ServiceIdToCountMap}}}";
        }
    }
}
