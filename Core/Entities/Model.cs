using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Model
    {
        public int Id { get; set; }

        public int BrandId { get; set; }
        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }

        public string Title { get; set; }

        public List<Car> Cars { get; set; }

        public override string ToString()
        {
            return $"{Title} {Brand}";
        }
    }
}
