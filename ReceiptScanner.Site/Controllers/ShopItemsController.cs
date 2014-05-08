using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ReceiptScanner.EF;

namespace ReceiptScanner.Site.Controllers
{
    public class ShopItemsController : ApiController
    {
        public IEnumerable<ShopItem> Get()
        {
            return new RSContext().ShopItems;
        }
    }
}
