using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ReceiptScanner.EF;
using Tesseract;

namespace ReceiptScanner.Parser
{
    public static class ReceiptParser
    {
        private const string DecimalRegexp = @"-?\d+(?:,\d\d+)?";
        private const string UnitRegexp = "[kg|ml|st]";

        internal enum ParseMode
        {
            ShopInfo,
            ShopItems,
            Summary
        }

        private static readonly Dictionary<char, char[]> Replacements = new Dictionary<char, char[]>();

        private static readonly Regex QuantityPriceRegexp;
        public static readonly Regex ShopItemRegexp;
        private static readonly Regex TotalRegexp;
        private static readonly Regex AccountNumberRegexp;
        private static readonly Regex DateTimeRegexp;

        static ReceiptParser()
        {
            AddReplacement('l', '1');
            AddReplacement('o', '0');
            AddReplacement('-', '\"');
            AddReplacement('*', 'w', 'r');
            AddReplacement('s', '5');

            QuantityPriceRegexp = CreateRegexp("(" + DecimalRegexp + ")" + UnitRegexp + "\\*(" + DecimalRegexp + ")Kr/" + UnitRegexp);
            ShopItemRegexp = CreateRegexp(string.Format(@"(.+)\s+(?:({0}){1}\*{0}\s+)?({0})$", DecimalRegexp, UnitRegexp));

            TotalRegexp = CreateRegexp(@"(?:TOTAL|Mottaget Kontokort)\s+(" + DecimalRegexp + ")");
            AccountNumberRegexp = CreateRegexp(@"KONTONUMMER:\s+(\d.*\d\d\d\d)");
            DateTimeRegexp = CreateRegexp(@"DATUM/TID\s+(.+)");
        }

        private static void AddReplacement(char c, params char[] parsedAs)
        {
            Replacements[c] = parsedAs;
        }

        public static Receipt Parse(byte[] image)
        {
            string datapath = AppDomain.CurrentDomain.BaseDirectory;
            var full = Path.Combine(datapath, "bin");
            var e = new TesseractEngine(full, "swe");
            var res = e.Process(new Bitmap(new MemoryStream(image)));
            var text = res.GetText();

            Receipt receiptInfo = ParseReceiptText(text);
            receiptInfo.ReceiptImage = image;
            return receiptInfo;
        }

        public static Receipt ParseReceiptText(string text)
        {
            var mode = ParseMode.ShopInfo;
            var receiptItems = new List<ReceiptItem>();
            var receiptInfo = new Receipt{ Items = receiptItems, Date = DateTime.Now };
            string prevUnmatchedLine = null;
            foreach (var line in text.Split('\n').Select(line => line.Trim()).Where(l => l != ""))
            {
                switch (mode)
                {
                    case ParseMode.ShopInfo:
                        if (receiptInfo.ShopName == null)
                        {
                            receiptInfo.ShopName = line;
                        }
                        else
                        {
                            if (ParseShopItem(line, receiptItems))
                                mode = ParseMode.ShopItems;
                        }
                        break;
                    case ParseMode.ShopItems:
                        if (ParseSummary(line, receiptInfo) || line.StartsWith("Moms%"))
                        {
                            mode = ParseMode.Summary;
                        }
                        else
                        {
                            if (!ParseShopItem(line, receiptItems))
                            {
                                prevUnmatchedLine = line;
                            }
                            else if (prevUnmatchedLine != null)
                            {
                                var item = receiptInfo.Items.Last();
                                var match = QuantityPriceRegexp.Match(item.Name);
                                if (match.Success)
                                {
                                    item.Quantity = ParseDecimal(match.Groups[1].Value);
                                    item.Name = prevUnmatchedLine;
                                }
                                prevUnmatchedLine = null;
                            }
                        }
                        break;
                    case ParseMode.Summary:
                        ParseSummary(line, receiptInfo);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return receiptInfo;
        }

        private static bool ParseSummary(string line, Receipt receiptInfo)
        {
            return
                ParseRegex(line, TotalRegexp, val => receiptInfo.Total = ParseDecimal(val)) ||
                ParseRegex(line, CreateRegexp(@"eco subtotal\s+(\d+,\d\d)"), val => { }) ||
                ParseRegex(line, AccountNumberRegexp, val => receiptInfo.AccountNumber = val) ||
                ParseRegex(line, DateTimeRegexp, val => receiptInfo.Date = DateTime.Parse(val));
        }

        private static Regex CreateRegexp(string regexp)
        {
            char? prevC = null;
            var fixedRegexp = "";
            foreach(var c in regexp)
            {
                string escape = Regex.Escape(c.ToString());
                bool isNormalCharacter = (escape.Length == 1 && prevC != '\\') || (escape.Length == 2 && prevC == '\\');
                if (isNormalCharacter && Replacements.ContainsKey(c))
                {
                    if (escape.Length == 2)
                    {
                        fixedRegexp = fixedRegexp.Remove(fixedRegexp.Length - 1);
                    }
                    var replaced = "[" + escape;
                    replaced = Replacements[c].Aggregate(replaced, (current, additionalCharacter) => current + ("|" + additionalCharacter));
                    replaced += "]";
                    fixedRegexp += replaced;
                }
                else
                {
                    fixedRegexp += c;
                }
                prevC = c;
            }
            return new Regex(fixedRegexp);
        }

        private static bool ParseRegex(string line, Regex regex, Action<string> func)
        {
            var match = regex.Match(line);
            if (match.Success) func(match.Groups[1].Value);
            return match.Success;
        }

        private static bool ParseShopItem(string line, ICollection<ReceiptItem> receiptItems)
        {
            var match = ShopItemRegexp.Match(line);
            if (!match.Success)
            {
                return false;
            }

            receiptItems.Add(new ReceiptItem
                {
                    Name = match.Groups[1].Value,
                    Price = ParseDecimal(match.Groups[3].Value),
                    Quantity = match.Groups[2].Success ? ParseDecimal(match.Groups[2].Value) : 1
                });
            return true;
        }

        private static decimal ParseDecimal(string priceString)
        {
            return decimal.Parse(priceString.Replace("\"", "-"));
        }
    }
}
