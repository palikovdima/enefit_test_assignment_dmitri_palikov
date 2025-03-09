using System.Collections.Generic;

namespace Domain.Entities
{
    public class Change
    {
        public Dictionary<string, ChangeItem> Bills { get; set; } = new();
        public Dictionary<string, ChangeItem> Coins { get; set; } = new();
    }

    public class ChangeItem
    {
        public int Count { get; set; }
        public string Image { get; set; } = "";
    }
}
