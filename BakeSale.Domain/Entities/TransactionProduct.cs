using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BakeSale.Domain.Entities.Common;

namespace BakeSale.Domain.Entities
{
    public class TransactionProduct : BaseEntity
    {
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }

}
