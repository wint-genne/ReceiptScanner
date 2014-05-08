using System.Data.Entity;

namespace ReceiptScanner.EF
{
    public class RSContext : DbContext
    {
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
    }
}