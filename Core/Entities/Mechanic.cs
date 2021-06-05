using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Mechanic : User
    {
        public List<Service> Services { get; set; } = new List<Service>();

        public override string ToString()
        {
            return base.ToString() + " mechanic";
        }
    }
}
