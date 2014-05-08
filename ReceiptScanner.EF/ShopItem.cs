using System.Collections.Generic;

namespace ReceiptScanner.EF
{
    public class ShopItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ReceiptItem> ReceiptItems { get; set; }
    }
}