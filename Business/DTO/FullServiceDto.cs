﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class FullServiceDto
    {
        public int ServiceId { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public UserDto Mechanic { get; set; }

        public FullServiceStatusDto status { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ServiceId)}={ServiceId.ToString()}, {nameof(Title)}={Title}, {nameof(Price)}={Price.ToString()}, {nameof(Mechanic)}={Mechanic}, {nameof(status)}={status}}}";
        }
    }
}
