using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ReceiptScanner.EF;
using ReceiptScanner.Parser;

namespace ReceiptScanner.Site.Controllers
{
    public class ReceiptsController : ApiController
    {
        public IEnumerable<Receipt> Get()
        {
            return new RSContext().Receipts;
        }

        public async Task<Receipt> Post()
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var rsContext = new RSContext();
            var file = provider.Contents.Single();
            var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
            var buffer = await file.ReadAsByteArrayAsync();
            var receipt = ReceiptParser.Parse(buffer);
            rsContext.Receipts.Add(receipt);
            rsContext.SaveChanges();
            return receipt;
        }

        public void Delete(int id)
        {
            var rsContext = new RSContext();
            rsContext.Receipts.Remove(rsContext.Receipts.Find(id));
            rsContext.SaveChanges();
        }
    }
}
