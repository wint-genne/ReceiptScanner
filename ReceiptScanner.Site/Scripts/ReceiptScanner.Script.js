(function() {
	'use strict';
	var $asm = {};
	global.ScriptProject = global.ScriptProject || {};
	ss.initAssembly($asm, 'ReceiptScanner.Script');
	////////////////////////////////////////////////////////////////////////////////
	// ScriptProject.KoMain
	var $ScriptProject_KoMain = function() {
		this.shopItems = ko.observableArray();
		this.receipts = ko.observableArray();
		this.receiptItems = ko.observableArray();
		this.selectedReceipt = ko.observable();
		$('#file-uploader').change(ss.mkdel(this, function(event) {
			var $t1 = $('#file-uploader').closest('form')[0];
			var formData = new FormData(ss.safeCast($t1, ss.isValue($t1) && (ss.isInstanceOfType($t1, Element) && $t1.tagName === 'FORM')));
			$.ajax({ url: '/api/receipts', type: 'POST', data: formData, success: ss.mkdel(this, function(data, status, request) {
				var receipt = data;
				this.receipts.push(receipt);
				this.selectedReceipt(receipt);
			}), cache: false, contentType: false, processData: false });
		}));
		$.get('/api/receipts', ss.mkdel(this, function(receipts) {
			this.receipts(ss.cast(receipts, Array));
		}));
		$.get('/api/shopItems', ss.mkdel(this, function(shopItems) {
			this.shopItems(ss.cast(shopItems, Array));
		}));
		ko.computed(ss.mkdel(this, function() {
			if (ss.isNullOrUndefined(this.selectedReceipt())) {
				return;
			}
			$.get('/api/receiptItems?receiptId=' + this.selectedReceipt().id, ss.mkdel(this, function(items) {
				this.receiptItems(ss.cast(items, Array));
			}));
		}));
		ko.applyBindings(this);
	};
	$ScriptProject_KoMain.__typeName = 'ScriptProject.KoMain';
	global.ScriptProject.KoMain = $ScriptProject_KoMain;
	////////////////////////////////////////////////////////////////////////////////
	// ScriptProject.Program
	var $ScriptProject_Program = function() {
	};
	$ScriptProject_Program.__typeName = 'ScriptProject.Program';
	$ScriptProject_Program.$main = function() {
		$(function() {
			new $ScriptProject_KoMain();
		});
	};
	global.ScriptProject.Program = $ScriptProject_Program;
	////////////////////////////////////////////////////////////////////////////////
	// ScriptProject.Receipt
	var $ScriptProject_Receipt = function() {
		this.id = 0;
	};
	$ScriptProject_Receipt.__typeName = 'ScriptProject.Receipt';
	global.ScriptProject.Receipt = $ScriptProject_Receipt;
	////////////////////////////////////////////////////////////////////////////////
	// ScriptProject.ReceiptItem
	var $ScriptProject_ReceiptItem = function() {
		this.$1$IdField = 0;
	};
	$ScriptProject_ReceiptItem.__typeName = 'ScriptProject.ReceiptItem';
	global.ScriptProject.ReceiptItem = $ScriptProject_ReceiptItem;
	////////////////////////////////////////////////////////////////////////////////
	// ScriptProject.ShopItem
	var $ScriptProject_ShopItem = function() {
	};
	$ScriptProject_ShopItem.__typeName = 'ScriptProject.ShopItem';
	global.ScriptProject.ShopItem = $ScriptProject_ShopItem;
	ss.initClass($ScriptProject_KoMain, $asm, {
		addReceiptItem: function() {
			$.post('/api/receiptItems', this.selectedReceipt().id, ss.mkdel(this, function(data) {
				this.receiptItems.push(data);
			}));
		},
		scanReceipt: function() {
			$('#file-uploader').click();
		},
		removeReceipt: function(receipt) {
			this.receipts.remove(receipt);
			$.ajax({ url: '/api/receipts/' + receipt.id, type: 'DELETE' });
			if (ss.referenceEquals(this.selectedReceipt(), receipt)) {
				this.selectedReceipt(null);
			}
		},
		saveReceiptItem: function(receiptItem) {
			$.ajax('/api/receiptItems' + receiptItem.get_id(), { type: 'PUT', data: ss.cast(receiptItem, Array) });
		}
	});
	ss.initClass($ScriptProject_Program, $asm, {});
	ss.initClass($ScriptProject_Receipt, $asm, {});
	ss.initClass($ScriptProject_ReceiptItem, $asm, {
		get_id: function() {
			return this.$1$IdField;
		},
		set_id: function(value) {
			this.$1$IdField = value;
		}
	});
	ss.initClass($ScriptProject_ShopItem, $asm, {});
	$ScriptProject_Program.$main();
})();
