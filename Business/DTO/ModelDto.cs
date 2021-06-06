using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class ModelDto
    {
        public int Id { get; set; }

        public int BrandId { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(BrandId)}={BrandId.ToString()}, {nameof(Title)}={Title}}}";
        }
    }
}
