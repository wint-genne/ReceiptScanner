using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceiptScanner.Parser;

namespace ReceiptScanner.Tests
{
    [TestClass]
    public class ReceiptParserTests
    {
        [TestMethod]
        public void TestPrimaLivs()
        {
            var parsedText = @"PRIMA LIVS AB

SÄVEDALEN TEL. 031-3408780

Saijare: 4022 Kassa: 01 Nr: 9972
Datum: 2014-04-27 Tid: 10:23
avooado 2str7,90 15,80
Biodgrapejuioe 15,90
Bärkasse papper 2,00
Barkasae piast 25t*1,5O 3,00
Creme Fraioha 34% 21,70
Faiukorv gourmet 31,90
Gourmaskinka rökt 31,90
Grekisk saiiadsost 18,90
Gurka ävensk
, 0,390kgw19,90Kr/kg 7,76
Haiioumi 27,90
Havretrippei 22,90
ICA Hush Matåkök 26,90
jordgubbar 2stx10,00 20,00
Kyckling 27,90
*Kyckiingfiié 90,00
Rabatt: Kyckiing 59,90kr ""30,10
Laxfiie 4-p ICA 95,00
Miid yogh nat EKO 2st*16,80 33,60
Paprika Röd
0,260kgw29,90Kr/kg 7,77
Potatis fast
1,220kgw8,90Kr/kg 10,86
Russin 29,90
Saiiads/Matbar
0,870kgw109,50Kr/kg 95,27
Satsumas
0,655kg*29,90Kr/kg 19,58
Sensod Tandkr 75m1 26,90
Shamp normait gul 31,90
Smoothie Yogh Apri 2st*11,90 23,80
Smoothie Yogh Jgb 3stw11,90 35,70
tomat körsbär 8,90
Torsk 59,00
V6 Duai 0oeanMint 2st«16,90 33,80
Viapgrädde 36% 20,70
Äppie Granny ICA
0,560kg*18,90Kr/kg 10,58
Äsens L.mjöik G.da 24,50
1'(›1:êa1 EBCJZZ. JLZZ b(r“
Moms% Moms Netto Brutto
12,00 86,90 724,52 811,42
25,00 18,14 72,56 90,70
Erháiien rabatt: 30,10
Mottaget Kontokort 902,12
Term:2651-065201 SwE:921452
VISA *xwxxwxxxxxx2605
27/04/2014 10:23 AID:A00O000003101O
TVR:0080001000 TSI:E800

Ref:074687904549 404 Rsp:00 611288 Cal 5
Personlig kod

Köp 902,12
Varav moms 105,04
Totait SEK 902,12
Spara kvittot

0RG.NR. 556624-2177
VÄLKOMMEN ATER
vid eventueiia rekiamationer
medtag aiitid vara och kvitto.

";

            var receipt = ReceiptParser.ParseReceiptText(parsedText);
            Assert.AreEqual((decimal)902.12, receipt.Total);

            var expected = new Tuple<string, double, double>[]
            {
                Tuple.Create("avooado", 2.0, 15.80),
                Tuple.Create("Biodgrapejuioe", 1.0, 15.90),
                Tuple.Create("Bärkasse papper", 1.0, 2.0),
                Tuple.Create("Barkasae piast", 2.0, 3.0),
                Tuple.Create("Creme Fraioha 34%", 1.0, 21.70),
                Tuple.Create("Faiukorv gourmet", 1.0, 31.90),
                Tuple.Create("Gourmaskinka rökt", 1.0, 31.90),
                Tuple.Create("Grekisk saiiadsost", 1.0, 18.90),
                Tuple.Create("Gurka ävensk", 0.39, 7.76),
                Tuple.Create("Haiioumi", 1.0, 27.90),
                Tuple.Create("Havretrippei", 1.0, 22.90),
                Tuple.Create("ICA Hush Matåkök", 1.0, 26.90),
                Tuple.Create("jordgubbar", 2.0, 20.00),
                Tuple.Create("Kyckling", 1.0, 27.90),
                Tuple.Create("*Kyckiingfiié", 1.0, 59.90),
                Tuple.Create("Laxfiie 4-p ICA", 1.0, 95.00),
                Tuple.Create("Miid yogh nat EKO", 2.0, 33.60),
                Tuple.Create("Paprika Röd", 0.26, 7.77),
                Tuple.Create("Potatis fast", 1.22, 10.86),
                Tuple.Create("Russin", 1.0, 29.90),
                Tuple.Create("Saiiads/Matbar", 0.87, 95.27),
                Tuple.Create("Satsumas", 0.655, 19.58),
                Tuple.Create("Sensod Tandkr 75m1", 1.0, 26.90),
                Tuple.Create("Shamp normait gul", 1.0, 31.90),
                Tuple.Create("Smoothie Yogh Apri", 2.0, 23.80),
                Tuple.Create("Smoothie Yogh Jgb", 3.0, 35.70),
                Tuple.Create("tomat körsbär", 1.0, 8.90),
                Tuple.Create("Torsk", 1.0, 59.00),
                Tuple.Create("V6 Duai 0oeanMint", 2.0, 33.80),
                Tuple.Create("Viapgrädde 36%", 1.0, 20.70),
                Tuple.Create("Äppie Granny ICA", 0.56, 10.58),
                Tuple.Create("Äsens L.mjöik G.da", 1.0, 24.50)
            };

            var i = 0;
            foreach(var receiptItem in receipt.Items)
            {
                Assert.AreEqual(expected[i].Item1, receiptItem.Name);
                Assert.AreEqual((decimal)expected[i].Item2, receiptItem.Quantity);
                Assert.AreEqual((decimal)expected[i].Item3, receiptItem.Price);
                i++;
            }
        }

        [TestMethod]
        [DeploymentItem("bild (1).jpg")]
        [DeploymentItem("x64", "x64")]
        [DeploymentItem("x86", "x86")]
        [DeploymentItem("tessdata", "bin\\tessdata")]
        public void TestMobilePhoto()
        {
            var photo = "bild (1).jpg";
            var receipt = ReceiptParser.Parse(File.ReadAllBytes(photo));
            Assert.IsTrue(receipt.Total > 0);
        }
    }
}
