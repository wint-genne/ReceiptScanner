using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tesseract;

namespace ReceiptScanner
{
    class Program
    {
        private static void Main(string[] args)
        {
            var cTempOcrtestPng = @"C:\Users\Christian\Google Drive\Scannat\IMG_20140427_0001.jpg";
        }
    }

    internal class ReceiptInfo
    {
        public string ShopName { get; set; }
        public string OrgNr { get; set; }
        public ICollection<ShopItem> Items { get; set; }

        public decimal Sum { get; set; }
        public string AccountNumber { get; set; }
        public string Date { get; set; }
    }

    internal class ShopItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public decimal? Quantity { get; set; }

        public override string ToString()
        {
            return Name + " " + Price;
        }
    }
}
