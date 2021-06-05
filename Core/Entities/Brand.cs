using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Brand
    {
        public int Id { get; set; } = 1;

        public string Title { get; set; }

        public List<Model> Models { get; set; }

        public override string ToString()
        {
            return $"{Title}";
        }
    }
}
