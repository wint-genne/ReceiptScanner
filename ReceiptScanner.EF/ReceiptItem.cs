using System.ComponentModel.DataAnnotations;

namespace ReceiptScanner.EF
{
    public class ReceiptItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

        public ShopItem ShopItem { get; set; }

        [Required]
        public Receipt Receipt { get; set; }
    }
}