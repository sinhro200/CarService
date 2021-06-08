using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Car
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User Owner{ get; set; }

        public int ModelId { get; set; }
        [ForeignKey("ModelId")]
        public Model Model { get; set; }

        public List<Order> Orders{ get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}={Id.ToString()}, {nameof(OwnerId)}={OwnerId.ToString()}, {nameof(Owner)}={Owner}, {nameof(ModelId)}={ModelId.ToString()}, {nameof(Model)}={Model}}}";
        }
    }
}
