using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ReceiptScanner.EF;

namespace ReceiptScanner.Site.Controllers
{
    public class ReceiptItemsController : ApiController
    {
        public IEnumerable<ReceiptItem> Get(int receiptId)
        {
            return new RSContext().Receipts.Where(r => r.Id == receiptId).SelectMany(r => r.Items).ToArray();
        }

        public void Put(int receiptId, ReceiptItem item)
        {
            var rsContext = new RSContext();
            var receiptItem = rsContext.ReceiptItems.Find(receiptId);
            receiptItem.Name = item.Name;
            receiptItem.Quantity = item.Quantity;
            receiptItem.Price = item.Price;
            rsContext.SaveChanges();
        }
    }
}
