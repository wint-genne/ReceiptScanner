using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReceiptScanner.EF
{
    public class ReceiptItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }

        public ShopItem ShopItem { get; set; }

        [ForeignKey("Receipt_Id")]
        [Required]
        public Receipt Receipt { get; set; }
        public int Receipt_Id { get; set; }
    }
}