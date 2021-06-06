﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(ServiceId)}={ServiceId.ToString()}, {nameof(Title)}={Title}}}";
        }
    }
}
