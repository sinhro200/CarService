using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class OrderFilterDto
    {
        public List<int> OwnerIds { get; set; }

        public List<int> CarModelIds { get; set; }

        public DateTime? CreationDateTimeMin { get; set; }

        public DateTime? CreationDateTimeMax { get; set; }

        public DateTime? ClosedDateTimeMin { get; set; }

        public DateTime? ClosedDateTimeMax { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(OwnerIds)}={OwnerIds}, {nameof(CarModelIds)}={CarModelIds}, {nameof(CreationDateTimeMin)}={CreationDateTimeMin.ToString()}, {nameof(CreationDateTimeMax)}={CreationDateTimeMax.ToString()}, {nameof(ClosedDateTimeMin)}={ClosedDateTimeMin.ToString()}, {nameof(ClosedDateTimeMax)}={ClosedDateTimeMax.ToString()}}}";
        }
    }
}
