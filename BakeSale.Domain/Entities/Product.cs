using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeSale.Domain.Entities.Common;

namespace BakeSale.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageSource { get; set; } = "";
    }
}
