using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptScanner.EF
{
    public class Receipt
    {
        public int Id { get; set; }
        public byte[] ReceiptImage { get; set; }
        public string ShopName { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public ICollection<ReceiptItem> Items { get; set; }
        public string AccountNumber { get; set; }
    }
}
