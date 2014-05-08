function ShopItem(receipt) {
}

function RSViewModel() {
    this.shopItems = ko.observableArray();
    this.receipts = ko.observableArray();
    this.receiptItems = ko.observableArray();
    this.selectedReceipt = ko.observable();

    this.scanReceipt = function() {
        $("#file-uploader").click();
    };

    this.removeReceipt = function(receipt) {
        this.receipts.remove(receipt);
        $.ajax({ url: "/api/receipts/" + receipt.id, method: "DELETE" });

        if (this.selectedReceipt() == receipt) this.selectedReceipt(null);
    };

    this.saveReceiptItem = function(receiptItem) {
        $.put("/api/receiptItems" + receiptItem.id, receiptItem);
    };

    var self = this;

    $("#file-uploader").change(function() {
        $.ajax(
        {
            url: "/api/receipts",
            type: 'POST',
            data: new FormData($("#file-uploader").closest("form")[0]),
            success: function (receipt) {
                self.receipts.push(receipt);
                self.selectedReceipt(receipt);
            },
            //Options to tell jQuery not to process data or worry about content-type.
            cache: false,
            contentType: false,
            processData: false
        });
    });

    $.get("/api/receipts", function(receipts) {
        self.receipts(receipts);
    });
    $.get("/api/shopItems", function(shopItems) {
        self.shopItems(shopItems);
    });

    ko.computed(function () {
        if (self.selectedReceipt() == null) return;
        $.get("/api/receiptItems?receiptId=" + self.selectedReceipt().id, function (items) {
            self.receiptItems(items);
        });
    });
}

// Initiate the Knockout bindings
ko.applyBindings(new RSViewModel());
