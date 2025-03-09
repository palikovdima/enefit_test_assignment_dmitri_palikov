using System;
using System.Collections.Generic;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public ICollection<TransactionProduct> ProductsSold { get; set; } = [];

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string ChangeJson { get; set; } = "";

        [NotMapped]
        public Change Change
        {
            get => string.IsNullOrEmpty(ChangeJson) ? new Change() : JsonSerializer.Deserialize<Change>(ChangeJson)!;
            set => ChangeJson = JsonSerializer.Serialize(value);
        }
    }
}
