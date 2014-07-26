using System;
using System.Collections.Generic;
using System.Html;
using System.Net;
using KnockoutApi;
using jQueryApi;

namespace ScriptProject
{
    public class KoMain
    {
        public ObservableArray<ShopItem> ShopItems = new ObservableArray<ShopItem>();
        public ObservableArray<Receipt> Receipts = new ObservableArray<Receipt>();
        public ObservableArray<ReceiptItem> ReceiptItems = new ObservableArray<ReceiptItem>();
        public Observable<Receipt> SelectedReceipt = new Observable<Receipt>();

        public KoMain()
        {
            jQuery.Select("#file-uploader").Change(@event =>
            {
                var formData = new FormData(jQuery.Select("#file-uploader").Closest("form")[0] as FormElement);
                jQuery.Ajax(new jQueryAjaxOptions
                {
                    Url = "/api/receipts",
                    Type = "POST",
                    Data = JsDictionary<string, object>.GetDictionary(formData),
                    Success = (data, status, request) =>
                    {
                        var receipt = Script.Reinterpret<Receipt>(data);
                        Receipts.Push(receipt);
                        SelectedReceipt.Value = receipt;
                    },
                    //Options to tell jQuery not to process data or worry about content-type.
                    Cache = false,
                    ContentType = Script.Reinterpret<string>(false),
                    ProcessData = false
                });
            });

            jQuery.Get("/api/receipts", receipts => {
                                                        Receipts.Value = (Receipt[]) receipts;
            });
            jQuery.Get("/api/shopItems", shopItems => {
                                                          this.ShopItems.Value = (ShopItem[]) shopItems;
            });

            Knockout.Computed(() =>
            {
                if (SelectedReceipt.Value == null) return;
                jQuery.Get("/api/receiptItems?receiptId=" + SelectedReceipt.Value.Id, items =>
                {
                    ReceiptItems.Value =
                        (ReceiptItem[])
                        items;
                });
            });

            Knockout.ApplyBindings(this);
        }

        public void AddReceiptItem()
        {
            jQuery.Post("/api/receiptItems", SelectedReceipt.Value.Id, data =>
            {
                ReceiptItems.Push(Script.Reinterpret<ReceiptItem>(data));
            });
        }

        public void ScanReceipt()
        {
            jQuery.Select("#file-uploader").Click();
        }

        public void RemoveReceipt(Receipt receipt)
        {
            Receipts.Remove(receipt);
            jQuery.Ajax(new jQueryAjaxOptions {Url = "/api/receipts/" + receipt.Id, Type = "DELETE"});

            if (SelectedReceipt.Value == receipt) SelectedReceipt.Value = null;
        }

        public void SaveReceiptItem(ReceiptItem receiptItem) 
        {
            jQuery.Ajax("/api/receiptItems" + receiptItem.Id, new jQueryAjaxOptions{ Type = "PUT", Data = (Array)(object)receiptItem });
        }
    
    }
}